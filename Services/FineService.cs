using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Fines;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class FineService : IFineService
    {
        private readonly ApplicationDbContext _context;

        public FineService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /fines — Staff/Admin xem danh sach phieu phat (UC14)
        public async Task<PaginationResponse<FineResponse>> GetFinesAsync(
            int page, int pageSize, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 50 ? 50 : pageSize;

            var query = _context.Fines
                .AsNoTracking()
                .Include(f => f.BorrowDetail)
                    .ThenInclude(bd => bd.BorrowRecord)
                        .ThenInclude(br => br.Reader);

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => MapToResponse(f))
                .ToListAsync(ct);

            return new PaginationResponse<FineResponse>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }

        // GET /fines/:id — Chi tiet phieu phat
        public async Task<FineResponse> GetFineByIdAsync(int fineId, CancellationToken ct = default)
        {
            var fine = await _context.Fines
                .AsNoTracking()
                .Include(f => f.BorrowDetail)
                    .ThenInclude(bd => bd.BorrowRecord)
                        .ThenInclude(br => br.Reader)
                .FirstOrDefaultAsync(f => f.FineId == fineId, ct);

            if (fine is null)
                throw new NotFoundException("Phieu phat khong ton tai!");

            return MapToResponse(fine);
        }

        // GET /users/:id/fines — Xem lich su vi pham cua nguoi dung (UC14)
        public async Task<PaginationResponse<FineResponse>> GetUserFinesAsync(
            int userId, int page, int pageSize, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 50 ? 50 : pageSize;

            var query = _context.Fines
                .AsNoTracking()
                .Include(f => f.BorrowDetail)
                    .ThenInclude(bd => bd.BorrowRecord)
                        .ThenInclude(br => br.Reader)
                .Where(f => f.BorrowDetail.BorrowRecord.ReaderId == userId);

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => MapToResponse(f))
                .ToListAsync(ct);

            return new PaginationResponse<FineResponse>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }

        // POST /fines — Staff tao phieu phat (UC14)
        public async Task<FineResponse> CreateFineAsync(CreateFineRequest request, CancellationToken ct = default)
        {
            // Kiem tra chi tiet muon ton tai
            var borrowDetail = await _context.BorrowDetails
                .Include(bd => bd.BorrowRecord)
                .FirstOrDefaultAsync(bd => bd.BorrowDetailId == request.BorrowDetailId, ct);

            if (borrowDetail is null)
                throw new NotFoundException("Chi tiet phieu muon khong ton tai!");

            // Kiem tra loai vi pham hop le
            var fine = new Fine
            {
                BorrowDetailId = request.BorrowDetailId,
                Amount = request.Amount,
                Reason = request.Reason,
                FineType = request.FineType,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            if (request.FineType == FineType.LOST)
            {
                await _context.BookCopies
                    .Where(bc => bc.CopyId == borrowDetail.CopyId)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(bc => bc.Status, BookCopyStatus.LOST), ct);
            }
            else if (request.FineType == FineType.DAMAGED ||
                     request.FineType == FineType.TORN)
            {
                await _context.BookCopies
                    .Where(bc => bc.CopyId == borrowDetail.CopyId)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(bc => bc.Status, BookCopyStatus.UNAVAILABLE), ct);
            }

            await _context.Fines.AddAsync(fine, ct);
            await _context.SaveChangesAsync(ct);

            return await GetFineByIdAsync(fine.FineId, ct);
        }

        // PATCH /fines/:id/pay — Staff xac nhan thanh toan (UC15)
        public async Task PayFineAsync(int fineId, CancellationToken ct = default)
        {
            var fine = await _context.Fines.FindAsync(new object[] { fineId }, ct);

            if (fine is null)
                throw new NotFoundException("Phieu phat khong ton tai!");

            if (fine.PaymentStatus == PaymentStatus.PAID)
                throw new BadRequestException("Phieu phat nay da duoc thanh toan!");

            fine.PaymentStatus = PaymentStatus.PAID;
            fine.PaidAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }

        // Private helper
        private static FineResponse MapToResponse(Fine f) => new()
        {
            FineId = f.FineId,
            BorrowDetailId = f.BorrowDetailId,
            BorrowCode = f.BorrowDetail?.BorrowRecord?.BorrowCode ?? string.Empty,
            ReaderName = f.BorrowDetail?.BorrowRecord?.Reader?.FullName ?? string.Empty,
            Amount = f.Amount,
            Reason = f.Reason,
            FineType = f.FineType.ToString(),
            PaymentStatus = f.PaymentStatus.ToString(),
            PaidAt = f.PaidAt,
            CreatedAt = f.CreatedAt
        };
    }
}