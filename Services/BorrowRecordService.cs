using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.BorrowRecords;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class BorrowRecordService : IBorrowRecordService
    {
        // Quy tac nghiep vu (UC08, UC11)
        private const int MaxBooksPerBorrow = 3;
        private const int BorrowDurationDays = 7;
        private const int ExtensionDays = 3;
        private const int MaxExtensions = 2;

        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public BorrowRecordService(
            ApplicationDbContext context,
            INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET /borrow-records — Staff/Admin (UC09)
        public async Task<PaginationResponse<BorrowRecordSummaryResponse>> GetBorrowRecordsAsync(
            BorrowRecordQueryRequest request, CancellationToken ct = default)
        {
            int page = request.Page < 1 ? 1 : request.Page;

            var query = _context.BorrowRecords
                .AsNoTracking()
                .Include(br => br.Reader);

            // Filter theo trang thai neu co
            IQueryable<BorrowRecord> filtered = query;
            if (request.Status.HasValue)
                filtered = filtered.Where(br => br.Status == request.Status.Value);

            int total = await filtered.CountAsync(ct);

            var items = await filtered
                .OrderByDescending(br => br.CreatedAt)
                .Skip((page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(br => new BorrowRecordSummaryResponse
                {
                    BorrowId = br.BorrowId,
                    BorrowCode = br.BorrowCode,
                    ReaderName = br.Reader.FullName,
                    BorrowDate = br.BorrowDate,
                    DueDate = br.DueDate,
                    Status = br.Status.ToString(),
                    ExtensionCount = br.ExtensionCount
                })
                .ToListAsync(ct);

            return new PaginationResponse<BorrowRecordSummaryResponse>
            {
                Items = items,
                Page = page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }

        // GET /users/:id/borrow-records — Lich su muon cua nguoi dung (UC09)
        public async Task<PaginationResponse<BorrowRecordSummaryResponse>> GetUserBorrowRecordsAsync(
            int userId, int page, int pageSize, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 50 ? 50 : pageSize;

            var query = _context.BorrowRecords
                .AsNoTracking()
                .Include(br => br.Reader)
                .Where(br => br.ReaderId == userId);

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(br => br.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(br => new BorrowRecordSummaryResponse
                {
                    BorrowId = br.BorrowId,
                    BorrowCode = br.BorrowCode,
                    ReaderName = br.Reader.FullName,
                    BorrowDate = br.BorrowDate,
                    DueDate = br.DueDate,
                    Status = br.Status.ToString(),
                    ExtensionCount = br.ExtensionCount
                })
                .ToListAsync(ct);

            return new PaginationResponse<BorrowRecordSummaryResponse>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }

        // GET /borrow-records/:id — Chi tiet phieu muon (UC10)
        public async Task<BorrowRecordDetailResponse> GetBorrowRecordByIdAsync(int borrowId, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .AsNoTracking()
                .Include(br => br.Reader)
                .Include(br => br.Approver)
                .Include(br => br.BorrowDetails)
                    .ThenInclude(bd => bd.BookCopy)
                        .ThenInclude(bc => bc.Book)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            if (record is null)
                throw new NotFoundException("Phieu muon khong ton tai!");

            return MapToDetailResponse(record);
        }

        // POST /borrow-records — Staff tao phieu muon moi (UC08)
        public async Task<BorrowRecordDetailResponse> CreateBorrowRecordAsync(
            int staffId, CreateBorrowRecordRequest request, CancellationToken ct = default)
        {
            // B4: Kiem tra trang thai tai khoan
            var reader = await _context.Users
                .Include(u => u.LibraryCard)
                .FirstOrDefaultAsync(u => u.UserId == request.ReaderId, ct);

            if (reader is null)
                throw new NotFoundException("Nguoi dung khong ton tai!");

            if (reader.Status == UserStatus.Locked)
                throw new ForbiddenException("Tai khoan nguoi dung da bi khoa!");

            if (reader.LibraryCard is null || reader.LibraryCard.Status != CardStatus.Active)
                throw new ForbiddenException("The thu vien khong hop le hoac da bi khoa!");

            // B7: Kiem tra so sach toi da
            if (request.CopyIds.Count > MaxBooksPerBorrow)
                throw new BadRequestException($"Chi duoc muon toi da {MaxBooksPerBorrow} cuon sach moi lan!");

            // Kiem tra so sach dang muon hien tai
            int currentBorrowing = await _context.BorrowRecords
                .Where(br => br.ReaderId == request.ReaderId
                    && br.Status == BorrowStatus.Borrowing)
                .SumAsync(br => br.BorrowDetails.Count(bd => bd.Status == BorrowDetailStatus.Borrowing), ct);

            if (currentBorrowing + request.CopyIds.Count > MaxBooksPerBorrow)
                throw new BadRequestException($"Nguoi dung da muon {currentBorrowing} sach. Khong the muon them!");

            // B6: Kiem tra ban sao ton tai va con san
            var copies = await _context.BookCopies
                .Where(bc => request.CopyIds.Contains(bc.CopyId))
                .ToListAsync(ct);

            if (copies.Count != request.CopyIds.Count)
                throw new NotFoundException("Mot so ban sao sach khong ton tai!");

            var unavailable = copies.Where(bc => bc.Status != BookCopyStatus.Available).ToList();
            if (unavailable.Any())
                throw new BadRequestException($"Mot so sach khong con san: {string.Join(", ", unavailable.Select(c => c.Barcode))}");

            // Tao phieu muon
            var borrowRecord = new BorrowRecord
            {
                ReaderId = request.ReaderId,
                ApprovedBy = staffId,
                BorrowCode = GenerateBorrowCode(),
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(BorrowDurationDays),
                ExtensionCount = 0,
                BorrowType = request.BorrowType,
                Status = BorrowStatus.Borrowing,
                ApprovedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                BorrowDetails = copies.Select(c => new BorrowDetail
                {
                    CopyId = c.CopyId,
                    Status = BorrowDetailStatus.Borrowing
                }).ToList()
            };

            await _context.BorrowRecords.AddAsync(borrowRecord, ct);

            // B10: Cap nhat trang thai ban sao -> Borrowed
            foreach (var copy in copies)
            {
                copy.Status = BookCopyStatus.Borrowed;
            }

            // Giam so luong kha dung tren bang books
            var bookIds = copies.Select(c => c.BookId).Distinct().ToList();
            foreach (var bookId in bookIds)
            {
                int count = copies.Count(c => c.BookId == bookId);
                await _context.Books
                    .Where(b => b.BookId == bookId)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(b => b.AvailabilityCopies, b => b.AvailabilityCopies - count), ct);
            }

            await _context.SaveChangesAsync(ct);

            return await GetBorrowRecordByIdAsync(borrowRecord.BorrowId, ct);
        }

        // PATCH /borrow-records/:id/return — Staff xac nhan tra sach (UC13)
        public async Task ConfirmReturnAsync(int borrowId, int staffId, ConfirmReturnRequest request, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .Include(br => br.BorrowDetails)
                    .ThenInclude(bd => bd.BookCopy)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            if (record is null)
                throw new NotFoundException("Phieu muon khong ton tai!");

            if (record.Status != BorrowStatus.Borrowing && record.Status != BorrowStatus.Overdue)
                throw new BadRequestException("Phieu muon khong o trang thai co the tra!");

            var now = DateTime.UtcNow;
            bool isOverdue = now > record.DueDate;

            // Cap nhat tung chi tiet muon
            foreach (var detail in record.BorrowDetails.Where(d => d.Status == BorrowDetailStatus.Borrowing))
            {
                // Tim trang thai tra tuong ung cho tung ban sao (neu co trong request)
                var returnItem = request.ReturnItems?.FirstOrDefault(r => r.CopyId == detail.CopyId);

                detail.ReturnedAt = now;
                detail.Status = returnItem?.Condition switch
                {
                    BookCondition.Torn => BorrowDetailStatus.Damaged,
                    BookCondition.Damaged => BorrowDetailStatus.Damaged,
                    _ => BorrowDetailStatus.Returned
                };
                detail.ItemCondition = returnItem?.Condition ?? BookCondition.Normal;

                // Cap nhat trang thai ban sao
                var copy = detail.BookCopy;
                copy.Status = detail.Status == BorrowDetailStatus.Damaged
                    ? BookCopyStatus.Damaged
                    : BookCopyStatus.Available;

                // Tang so luong sach kha dung neu tra binh thuong
                if (copy.Status == BookCopyStatus.Available)
                {
                    await _context.Books
                        .Where(b => b.BookId == copy.BookId)
                        .ExecuteUpdateAsync(s =>
                            s.SetProperty(b => b.AvailabilityCopies, b => b.AvailabilityCopies + 1), ct);
                }
            }

            record.Status = BorrowStatus.Returned;
            record.ReturnedDate = now;

            await _context.SaveChangesAsync(ct);

            // Gui thong bao tra sach thanh cong
            await _notificationService.SendAsync(
                record.ReaderId,
                "Tra sach thanh cong",
                $"Phieu muon {record.BorrowCode} da duoc xac nhan tra thanh cong.", ct);
        }

        // PATCH /borrow-records/:id/cancel — Huy phieu muon (UC12)
        public async Task CancelBorrowRecordAsync(int borrowId, int currentUserId, string currentRole, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .Include(br => br.BorrowDetails)
                    .ThenInclude(bd => bd.BookCopy)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            if (record is null)
                throw new NotFoundException("Phieu muon khong ton tai!");

            // Reader chi duoc huy phieu cua chinh minh
            if (currentRole == RoleName.READER.ToString() && record.ReaderId != currentUserId)
                throw new ForbiddenException("Ban khong co quyen huy phieu muon nay!");

            if (record.Status != BorrowStatus.Pending && record.Status != BorrowStatus.Borrowing)
                throw new BadRequestException("Khong the huy phieu muon o trang thai hien tai!");

            // Tra lai ban sao ve Available neu dang Borrowing
            if (record.Status == BorrowStatus.Borrowing)
            {
                foreach (var detail in record.BorrowDetails.Where(d => d.Status == BorrowDetailStatus.Borrowing))
                {
                    detail.Status = BorrowDetailStatus.Returned;
                    detail.BookCopy.Status = BookCopyStatus.Available;

                    await _context.Books
                        .Where(b => b.BookId == detail.BookCopy.BookId)
                        .ExecuteUpdateAsync(s =>
                            s.SetProperty(b => b.AvailabilityCopies, b => b.AvailabilityCopies + 1), ct);
                }
            }

            record.Status = BorrowStatus.Cancelled;

            await _context.SaveChangesAsync(ct);
        }

        // POST /borrow-records/:id/extension-requests — Reader gui yeu cau gia han (UC11)
        public async Task SubmitExtensionRequestAsync(int borrowId, int readerId, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId && br.ReaderId == readerId, ct);

            if (record is null)
                throw new NotFoundException("Phieu muon khong ton tai!");

            ValidateExtensionEligibility(record);

            // Danh dau trang thai cho Staff duyet — dung gia tri de phan biet
            // (Co the mo rong thanh bang ExtensionRequest rieng neu can)
            // Hien tai: luu ghi chu vao Notification de Staff xu ly
            await _notificationService.SendToStaffAsync(
                $"Yeu cau gia han phieu muon {record.BorrowCode} tu nguoi dung {readerId}.",
                ct);
        }

        // PATCH /borrow-records/:id/extend — Staff duyet gia han (UC11)
        public async Task ConfirmExtensionAsync(int borrowId, int staffId, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            if (record is null)
                throw new NotFoundException("Phieu muon khong ton tai!");

            ValidateExtensionEligibility(record);

            record.ExtensionCount += 1;
            record.DueDate = record.DueDate.AddDays(ExtensionDays);

            await _context.SaveChangesAsync(ct);

            // Gui thong bao gia han thanh cong
            await _notificationService.SendAsync(
                record.ReaderId,
                "Gia han sach thanh cong",
                $"Phieu muon {record.BorrowCode} da duoc gia han. Han tra moi: {record.DueDate:dd/MM/yyyy}.", ct);
        }

        // Private helpers

        private static void ValidateExtensionEligibility(BorrowRecord record)
        {
            if (record.Status != BorrowStatus.Borrowing)
                throw new BadRequestException("Phieu muon khong o trang thai dang muon!");

            if (record.ExtensionCount >= MaxExtensions)
                throw new BadRequestException($"Da vuot so lan gia han toi da ({MaxExtensions} lan)!");

            if (DateTime.UtcNow > record.DueDate)
                throw new BadRequestException("Sach da qua han, khong the gia han!");
        }

        private static BorrowRecordDetailResponse MapToDetailResponse(BorrowRecord br) => new()
        {
            BorrowId = br.BorrowId,
            BorrowCode = br.BorrowCode,
            ReaderId = br.ReaderId,
            ReaderName = br.Reader?.FullName ?? string.Empty,
            ApproverName = br.Approver?.FullName,
            BorrowDate = br.BorrowDate,
            DueDate = br.DueDate,
            ReturnedDate = br.ReturnedDate,
            ExtensionCount = br.ExtensionCount,
            BorrowType = br.BorrowType.ToString(),
            Status = br.Status.ToString(),
            CreatedAt = br.CreatedAt,
            Items = br.BorrowDetails.Select(d => new BorrowDetailItemResponse
            {
                BorrowDetailId = d.BorrowDetailId,
                CopyId = d.CopyId,
                Barcode = d.BookCopy?.Barcode ?? string.Empty,
                BookTitle = d.BookCopy?.Book?.Title ?? string.Empty,
                ReturnedAt = d.ReturnedAt,
                ItemCondition = d.ItemCondition?.ToString(),
                Status = d.Status.ToString()
            }).ToList()
        };

        private static string GenerateBorrowCode()
            => $"BR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").ToUpper()[..6]}";
    }
}