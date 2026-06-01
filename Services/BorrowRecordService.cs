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
        private readonly IHttpContextAccessor _contextAccessor;
        public BorrowRecordService(
            ApplicationDbContext context,
            INotificationService notificationService,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _notificationService = notificationService;
            _contextAccessor = contextAccessor;
        }

        // GET /borrow-records — Staff/Admin
        public async Task<PaginationResponse<BorrowRecordSummaryResponse>> GetBorrowRecordsAsync(
            BorrowRecordQueryRequest request, CancellationToken ct = default)
        {
            IQueryable<BorrowRecord> Query = _context.BorrowRecords.AsNoTracking();

            // Filter theo ma phieu
            if (!string.IsNullOrWhiteSpace(request.BorrowCode))
            {
                Query = Query.Where(br =>
                    br.BorrowCode.Contains(request.BorrowCode));    
            }

            // Filter theo ten nguoi doc
            if (!string.IsNullOrWhiteSpace(request.ReaderName))
            {
                Query = Query.Where(br =>
                    br.Reader.FullName.Contains(request.ReaderName));
            }
            // Filter theo trang thai
            if (request.Status.HasValue)
            {
                Query = Query.Where(br =>
                    br.Status == request.Status.Value
                );
            }

            int totalRecords = await Query.CountAsync(ct);

            var items = await Query
                .OrderByDescending(br => br.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(br => new BorrowRecordSummaryResponse
                {
                    BorrowId = br.BorrowId,
                    BorrowCode = br.BorrowCode,
                    ReaderId = br.ReaderId,
                    ReaderName = br.Reader.FullName,
                    BorrowDate = br.BorrowDate,
                    DueDate = br.DueDate,
                    ReturnedDate = br.ReturnedDate,
                    BorrowType = br.BorrowType.ToString(),
                    Status = br.Status.ToString(),
                    ExtensionCount = br.ExtensionCount,
                    TotalBooks = br.BorrowDetails.Count()
                }).ToListAsync(ct);

            return new PaginationResponse<BorrowRecordSummaryResponse>
            {
                Items = items,
                TotalRecords = totalRecords,
                PageSize = request.PageSize,
                Page = request.Page,
            };
        }

        // GET /users/:id/borrow-records — Lich su muon cua nguoi dung
        public async Task<PaginationResponse<BorrowRecordSummaryResponse>> GetUserBorrowRecordsAsync(int userId, PaginationRequest request, CancellationToken ct = default)
        {
            var currentUserId = int.Parse(
        _contextAccessor.HttpContext!
            .User.FindFirst("UserId")!.Value);

            var currentRole =
                _contextAccessor.HttpContext!
                    .User.FindFirst(System.Security.Claims.ClaimTypes.Role)!
                    .Value;

            IQueryable<BorrowRecord> query = _context.BorrowRecords
                .AsNoTracking();

            // Reader chỉ xem dữ liệu của chính mình
            if (currentRole == RoleName.READER.ToString())
            {
                if (currentUserId != userId)
                {
                    throw new ForbiddenException(
                        "Ban khong co quyen xem lich su muon cua nguoi dung khac!");
                }

                query = query.Where(br => br.ReaderId == currentUserId);
            }
            else
            {
                // Staff/Admin xem theo userId được truyền vào
                query = query.Where(br => br.ReaderId == userId);
            }

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(br => br.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(br => new BorrowRecordSummaryResponse{
                    BorrowId = br.BorrowId,
                    BorrowCode = br.BorrowCode,
                    ReaderId = br.ReaderId,
                    ReaderName = br.Reader.FullName,
                    BorrowDate = br.BorrowDate,
                    DueDate = br.DueDate,
                    ReturnedDate = br.ReturnedDate,
                    Status = br.Status.ToString(),
                    BorrowType = br.BorrowType.ToString(),
                    ExtensionCount = br.ExtensionCount,
                    TotalBooks = br.BorrowDetails.Count
                })
                .ToListAsync(ct);

            return new PaginationResponse<BorrowRecordSummaryResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }

        // GET /borrow-records/:id — Chi tiet phieu muon
        public async Task<BorrowRecordDetailResponse> GetBorrowRecordByIdAsync(int borrowId, CancellationToken ct = default)
        {
            //var record = await _context.BorrowRecords
            //    .AsNoTracking()
            //    .Include(br => br.Reader)
            //    .Include(br => br.Approver)
            //    .Include(br => br.BorrowDetails)
            //        .ThenInclude(bd => bd.BookCopy)
            //            .ThenInclude(bc => bc.Book)
            //    .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            //if (record is null)
            //    throw new NotFoundException("Phieu muon khong ton tai!");

            //return MapToDetailResponse(record);
        }

        // POST /borrow-records — Staff tao phieu muon moi
        public async Task<BorrowRecordDetailResponse> CreateBorrowRecordAsync(
            int staffId, CreateBorrowRecordRequest request, CancellationToken ct = default)
        {
            // B4: Kiem tra trang thai tai khoan
            //var reader = await _context.Users
            //    .Include(u => u.LibraryCard)
            //    .FirstOrDefaultAsync(u => u.UserId == request.ReaderId, ct);

            //if (reader is null)
            //    throw new NotFoundException("Nguoi dung khong ton tai!");

            //if (reader.Status == UserStatus.Locked)
            //    throw new ForbiddenException("Tai khoan nguoi dung da bi khoa!");

            //if (reader.LibraryCard is null || reader.LibraryCard.Status != CardStatus.Active)
            //    throw new ForbiddenException("The thu vien khong hop le hoac da bi khoa!");

            //// B7: Kiem tra so sach toi da
            //if (request.CopyIds.Count > MaxBooksPerBorrow)
            //    throw new BadRequestException($"Chi duoc muon toi da {MaxBooksPerBorrow} cuon sach moi lan!");

            //// Kiem tra so sach dang muon hien tai
            //int currentBorrowing = await _context.BorrowRecords
            //    .Where(br => br.ReaderId == request.ReaderId
            //        && br.Status == BorrowStatus.Borrowing)
            //    .SumAsync(br => br.BorrowDetails.Count(bd => bd.Status == BorrowDetailStatus.Borrowing), ct);

            //if (currentBorrowing + request.CopyIds.Count > MaxBooksPerBorrow)
            //    throw new BadRequestException($"Nguoi dung da muon {currentBorrowing} sach. Khong the muon them!");

            //// B6: Kiem tra ban sao ton tai va con san
            //var copies = await _context.BookCopies
            //    .Where(bc => request.CopyIds.Contains(bc.CopyId))
            //    .ToListAsync(ct);

            //if (copies.Count != request.CopyIds.Count)
            //    throw new NotFoundException("Mot so ban sao sach khong ton tai!");

            //var unavailable = copies.Where(bc => bc.Status != BookCopyStatus.Available).ToList();
            //if (unavailable.Any())
            //    throw new BadRequestException($"Mot so sach khong con san: {string.Join(", ", unavailable.Select(c => c.Barcode))}");

            //// Tao phieu muon
            //var borrowRecord = new BorrowRecord
            //{
            //    ReaderId = request.ReaderId,
            //    ApprovedBy = staffId,
            //    BorrowCode = GenerateBorrowCode(),
            //    BorrowDate = DateTime.UtcNow,
            //    DueDate = DateTime.UtcNow.AddDays(BorrowDurationDays),
            //    ExtensionCount = 0,
            //    BorrowType = request.BorrowType,
            //    Status = BorrowStatus.Borrowing,
            //    ApprovedAt = DateTime.UtcNow,
            //    CreatedAt = DateTime.UtcNow,
            //    BorrowDetails = copies.Select(c => new BorrowDetail
            //    {
            //        CopyId = c.CopyId,
            //        Status = BorrowDetailStatus.Borrowing
            //    }).ToList()
            //};

            //await _context.BorrowRecords.AddAsync(borrowRecord, ct);

            //// B10: Cap nhat trang thai ban sao -> Borrowed
            //foreach (var copy in copies)
            //{
            //    copy.Status = BookCopyStatus.Borrowed;
            //}

            //// Giam so luong kha dung tren bang books
            //var bookIds = copies.Select(c => c.BookId).Distinct().ToList();
            //foreach (var bookId in bookIds)
            //{
            //    int count = copies.Count(c => c.BookId == bookId);
            //    await _context.Books
            //        .Where(b => b.BookId == bookId)
            //        .ExecuteUpdateAsync(s =>
            //            s.SetProperty(b => b.AvailabilityCopies, b => b.AvailabilityCopies - count), ct);
            //}

            //await _context.SaveChangesAsync(ct);

            //return await GetBorrowRecordByIdAsync(borrowRecord.BorrowId, ct);
        }

        // | PATCH | /borrow-records/:id/return | Xác nhận trả sách | Staff/Admin |
        public async Task ConfirmReturnAsync(
           int borrowId,
           ConfirmReturnRequest request,
           CancellationToken ct = default)
        {
            // Lấy phiếu mượn kèm danh sách sách đã mượn 
            // và thông tin bản sao sách
            var record = await _context.BorrowRecords
                .Include(br => br.BorrowDetails)
                    .ThenInclude(bd => bd.BookCopy)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            // Kiểm tra phiếu mượn có tồn tại không
            if (record == null)
                throw new NotFoundException("Phieu muon khong ton tai!");

            // Chỉ cho phép xác nhận trả khi phiếu 
            // đang ở trạng thái BORROWING hoặc OVERDUE
            if (record.Status != BorrowStatus.BORROWING &&
                record.Status != BorrowStatus.OVERDUE)
                throw new BadRequestException("Phieu muon khong hop le!");

            // Kiểm tra đã trả đầy đủ số lượng sách chưa
            if (request.ReturnItems.Count != record.BorrowDetails.Count)
                throw new BadRequestException("Chua tra day du sach!");

            // Thời gian trả sách
            var now = DateTime.UtcNow;

            // Duyệt từng cuốn sách trong phiếu mượn
            foreach (var detail in record.BorrowDetails)
            {
                // Tìm thông tin trả sách tương ứng
                var returnItem = request.ReturnItems
                    .FirstOrDefault(x => x.CopyId == detail.CopyId);

                // Nếu không truyền condition thì mặc định NORMAL
                var condition = returnItem?.Condition ?? BookCondition.NORMAL;

                // Cập nhật thời gian trả và tình trạng sách
                detail.ReturnedAt = now;
                detail.ItemCondition = condition;

                // Cập nhật tình trạng của bản sao sách
                detail.BookCopy.Condition = condition;

                switch (condition)
                {
                    // Sách bình thường
                    case BookCondition.NORMAL:
                        detail.Status = BorrowDetailStatus.RETURNED;
                        detail.BookCopy.Status = BookCopyStatus.AVAILABLE;
                        break;

                    // Sách rách hoặc hỏng
                    case BookCondition.TORN:
                    case BookCondition.DAMAGED:
                        detail.Status = BorrowDetailStatus.DAMAGED;
                        // Chờ staff xem xét, tạm ngưng lưu hành
                        detail.BookCopy.Status = BookCopyStatus.UNAVAILABLE;

                        await _context.Fines.AddAsync(new Fine
                        {
                            BorrowDetailId = detail.BorrowDetailId,
                            FineType = FineType.DAMAGED,
                            PaymentStatus = PaymentStatus.PENDING,
                            CreatedAt = now
                        }, ct);
                        break;

                    // Sách bị mất
                    case BookCondition.LOST:
                        // Đánh dấu chi tiết mượn là mất
                        detail.Status = BorrowDetailStatus.LOST;
                        // Bản sao sách chuyển sang LOST
                        detail.BookCopy.Status = BookCopyStatus.LOST;
                        // Tạo phiếu phạt mất sách
                        await _context.Fines.AddAsync(new Fine
                        {
                            BorrowDetailId = detail.BorrowDetailId,
                            FineType = FineType.LOST,
                            PaymentStatus = PaymentStatus.PENDING,
                            CreatedAt = now
                        }, ct);

                        break;
                }
            }

            // Kiểm tra trả sách quá hạn
            if (now > record.DueDate)
            {
                // Tạo phiếu phạt quá hạn cho từng cuốn sách
                foreach (var detail in record.BorrowDetails)
                {
                    await _context.Fines.AddAsync(new Fine
                    {
                        BorrowDetailId = detail.BorrowDetailId,
                        FineType = FineType.OVERDUE,
                        PaymentStatus = PaymentStatus.PENDING,
                        CreatedAt = now
                    }, ct);
                }
            }

            // Cập nhật trạng thái phiếu mượn
            record.Status = BorrowStatus.RETURNED;
            record.ReturnedDate = now;

            // Lưu thay đổi xuống database
            await _context.SaveChangesAsync(ct);
        }
        // PATCH /borrow-records/:id/cancel — Huy phieu muon
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

            if (record.Status != BorrowStatus.PENDING && record.Status != BorrowStatus.BORROWING)
                throw new BadRequestException("Khong the huy phieu muon o trang thai hien tai!");

            // Tra lai ban sao ve Available neu dang Borrowing
            if (record.Status == BorrowStatus.BORROWING)
            {
                foreach (var detail in record.BorrowDetails.Where(d => d.Status == BorrowDetailStatus.BORROWING))
                {
                    detail.Status = BorrowDetailStatus.RETURNED;
                    detail.BookCopy.Status = BookCopyStatus.AVAILABLE;

                    await _context.Books
                        .Where(b => b.BookId == detail.BookCopy.BookId)
                        .ExecuteUpdateAsync(s =>
                            s.SetProperty(b => b.AvailabilityCopies, b => b.AvailabilityCopies + 1), ct);
                }
            }

            record.Status = BorrowStatus.CANCELLED;

            await _context.SaveChangesAsync(ct);
        }

        // POST /borrow-records/:id/extension-requests — Reader gui yeu cau gia han
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

        // PATCH /borrow-records/:id/extend — Staff duyet gia han
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

        private static string GenerateBorrowCode()
            => $"BR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").ToUpper()[..6]}";
    }
}