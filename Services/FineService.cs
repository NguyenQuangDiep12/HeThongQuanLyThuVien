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


        // | GET | /fines | Danh sách phiếu phạt | Staff/Admin |
        public async Task<PaginationResponse<FineResponse>> GetFinesAsync(PaginationRequest request,CancellationToken ct = default)
        {
            var query = _context.Fines
                .AsNoTracking();

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(f => new FineResponse
                {
                    FineId = f.FineId,
                    BorrowDetailId = f.BorrowDetailId,
                    BorrowCode = f.BorrowDetail.BorrowRecord.BorrowCode,
                    ReaderId = f.BorrowDetail.BorrowRecord.ReaderId,
                    ReaderName = f.BorrowDetail.BorrowRecord.Reader.FullName,
                    Amount = f.Amount,
                    Reason = f.Reason,
                    FineType = f.FineType,
                    PaymentStatus = f.PaymentStatus,
                    PaidAt = f.PaidAt,
                    CreatedAt = f.CreatedAt
                }).ToListAsync(ct);

            return new PaginationResponse<FineResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }

        // | GET | /fines/:id | Chi tiết phiếu phạt | Staff/Admin |
        public async Task<FineResponse> GetFineByIdAsync(int fineId, CancellationToken ct = default)
        {
            var fine = await _context.Fines.AsNoTracking().Where(f => f.FineId == fineId)
                .Select(f => new FineResponse
                {
                    FineId = f.FineId,
                    BorrowDetailId = f.BorrowDetailId,
                    BorrowCode = f.BorrowDetail
                        .BorrowRecord
                        .BorrowCode,
                    ReaderId = f.BorrowDetail
                        .BorrowRecord
                        .ReaderId,
                    ReaderName = f.BorrowDetail
                        .BorrowRecord
                        .Reader
                        .FullName,
                    Amount = f.Amount,
                    Reason = f.Reason,
                    FineType = f.FineType,
                    PaymentStatus = f.PaymentStatus,
                    PaidAt = f.PaidAt,
                    CreatedAt = f.CreatedAt
                }).FirstOrDefaultAsync(ct);

            if (fine is null)throw new NotFoundException("Phieu phat khong ton tai!");
            return fine;
        }
        // | GET | /users/:id/fines | Theo dõi vi phạm của người dùng | Staff/Admin |
        public async Task<PaginationResponse<FineResponse>> GetUserFinesAsync(int userId,PaginationRequest request,CancellationToken ct = default)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId, ct);

            if (!userExists) throw new NotFoundException("Nguoi dung khong ton tai!");

            var query = _context.Fines.AsNoTracking().Where(f => f.BorrowDetail.BorrowRecord.ReaderId == userId);

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(f => new FineResponse
                {
                    FineId = f.FineId,
                    BorrowDetailId = f.BorrowDetailId,
                    BorrowCode = f.BorrowDetail.BorrowRecord.BorrowCode,
                    ReaderId = f.BorrowDetail.BorrowRecord.ReaderId,
                    ReaderName = f.BorrowDetail.BorrowRecord.Reader.FullName,
                    Amount = f.Amount,
                    Reason = f.Reason,
                    FineType = f.FineType,
                    PaymentStatus = f.PaymentStatus,
                    PaidAt = f.PaidAt,
                    CreatedAt = f.CreatedAt
                }).ToListAsync(ct);
            return new PaginationResponse<FineResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }

        // | POST | /fines | Tạo phiếu phạt | Staff/Admin |
        public async Task<FineResponse> CreateFineAsync(CreateFineRequest request,CancellationToken ct = default)
        {
            var borrowDetail = await _context.BorrowDetails
                .Include(bd => bd.BorrowRecord)
                .Include(bd => bd.BookCopy)
                .FirstOrDefaultAsync(bd => bd.BorrowDetailId == request.BorrowDetailId, ct);
            if (borrowDetail is null)throw new NotFoundException("Chi tiet phieu muon khong ton tai!");
            bool fineExists = await _context.Fines.AnyAsync(f => f.BorrowDetailId == request.BorrowDetailId, ct);
            if (fineExists) throw new BadRequestException( "Chi tiet muon nay da ton tai phieu phat!");

            switch (request.FineType)
            {
                case FineType.OVERDUE:

                    if (borrowDetail.ReturnedAt is null)
                    {
                        throw new BadRequestException("Sach chua duoc tra!");
                    }

                    if (borrowDetail.ReturnedAt.Value <= borrowDetail.BorrowRecord.DueDate)
                    {
                        throw new BadRequestException("Sach khong bi qua han!");
                    }
                    break;
                case FineType.LOST:
                    if (borrowDetail.BookCopy.Status == BookCopyStatus.LOST)
                    {
                        throw new BadRequestException("Ban sao sach da duoc danh dau mat!");
                    }
                    borrowDetail.BookCopy.Status = BookCopyStatus.LOST;
                    break;
                case FineType.DAMAGED:
                    borrowDetail.BookCopy.Status = BookCopyStatus.UNAVAILABLE;
                    break;
                default:
                    throw new BadRequestException("Loai vi pham khong hop le!");
            }

            var fine = new Fine
            {
                BorrowDetailId = request.BorrowDetailId,
                Amount = request.Amount,
                Reason = request.Reason,
                FineType = request.FineType,
                PaymentStatus = PaymentStatus.PENDING,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Fines.AddAsync(fine, ct);
            await _context.SaveChangesAsync(ct);
            return await GetFineByIdAsync(fine.FineId, ct);
        }
        // | PATCH | /fines/:id/pay | Xác nhận đã thanh toán | Staff/Admin |
        public async Task PayFineAsync(int fineId, CancellationToken ct = default)
        {
            var fine = await _context.Fines.FirstOrDefaultAsync(f => f.FineId == fineId, ct);
            if (fine is null)
                throw new NotFoundException("Phieu phat khong ton tai!");
            if (fine.PaymentStatus == PaymentStatus.PAID)
                throw new BadRequestException("Phieu phat da duoc thanh toan!");
            fine.PaymentStatus = PaymentStatus.PAID;
            fine.PaidAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }
}
