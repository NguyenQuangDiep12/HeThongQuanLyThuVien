using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.BorrowRecords;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Options;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HeThongQuanLyThuVien.Services
{
    public class BorrowRecordService : IBorrowRecordService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly LibrarySettings _librarySettings;

        public BorrowRecordService(
            ApplicationDbContext context,
            INotificationService notificationService,
            IOptions<LibrarySettings> librarySettings)
        {
            _context = context;
            _notificationService = notificationService;
            _librarySettings = librarySettings.Value;
        }

        // GET /borrow-records — Staff/Admin
        public async Task<PaginationResponse<BorrowRecordSummaryResponse>> GetBorrowRecordsAsync(
            BorrowRecordQueryRequest request,
            CancellationToken ct = default)
        {
            IQueryable<BorrowRecord> query = _context.BorrowRecords.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.BorrowCode))
                query = query.Where(br => br.BorrowCode.Contains(request.BorrowCode));

            if (!string.IsNullOrWhiteSpace(request.ReaderName))
                query = query.Where(br => br.Reader.FullName.Contains(request.ReaderName));

            if (request.Status.HasValue)
                query = query.Where(br => br.Status == request.Status.Value);

            // Filter theo trạng thái yêu cầu gia hạn
            // Staff dùng ?extensionRequestStatus=PENDING để thấy danh sách chờ duyệt
            if (request.ExtensionRequestStatus.HasValue)
                query = query.Where(br => br.ExtensionRequestStatus == request.ExtensionRequestStatus.Value);

            int totalRecords = await query.CountAsync(ct);

            var items = await query
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
                    ExtensionRequestStatus = br.ExtensionRequestStatus.ToString(),
                    TotalBooks = br.BorrowDetails.Count()
                })
                .ToListAsync(ct);

            return new PaginationResponse<BorrowRecordSummaryResponse>
            {
                Items = items,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        // GET /users/:id/borrow-records
        public async Task<PaginationResponse<BorrowRecordSummaryResponse>> GetUserBorrowRecordsAsync(
            int userId,
            int currentUserId,
            string currentRole,
            PaginationRequest request,
            CancellationToken ct = default)
        {
            IQueryable<BorrowRecord> query = _context.BorrowRecords.AsNoTracking();

            if (currentRole == RoleName.READER.ToString())
            {
                if (currentUserId != userId)
                    throw new ForbiddenException("Bạn không có quyền xem lịch sử mượn của người khác!");

                query = query.Where(br => br.ReaderId == currentUserId);
            }
            else
            {
                query = query.Where(br => br.ReaderId == userId);
            }

            int totalRecords = await query.CountAsync(ct);

            var items = await query
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
                    ExtensionRequestStatus = br.ExtensionRequestStatus.ToString(),
                    TotalBooks = br.BorrowDetails.Count()
                })
                .ToListAsync(ct);

            return new PaginationResponse<BorrowRecordSummaryResponse>
            {
                Items = items,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        // GET /borrow-records/:id
        public async Task<BorrowRecordDetailResponse> GetBorrowRecordByIdAsync(
            int borrowId,
            int currentUserId,
            string currentRole,
            CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .AsNoTracking()
                .Where(br => br.BorrowId == borrowId)
                .Select(br => new BorrowRecordDetailResponse
                {
                    BorrowId = br.BorrowId,
                    BorrowCode = br.BorrowCode,
                    ReaderId = br.ReaderId,
                    ReaderName = br.Reader.FullName,
                    ApproverId = br.ApprovedBy,
                    ApproverName = br.Approver != null ? br.Approver.FullName : null,
                    BorrowDate = br.BorrowDate,
                    DueDate = br.DueDate,
                    ReturnedDate = br.ReturnedDate,
                    BorrowType = br.BorrowType,
                    Status = br.Status,
                    ExtensionCount = br.ExtensionCount,
                    BorrowDetails = br.BorrowDetails
                        .Select(bd => new BorrowDetailResponse
                        {
                            BorrowDetailId = bd.BorrowDetailId,
                            CopyId = bd.CopyId,
                            Barcode = bd.BookCopy.Barcode,
                            BookTitle = bd.BookCopy.Book.Title,
                            ReturnedAt = bd.ReturnedAt,
                            ItemCondition = bd.ItemCondition.HasValue ? bd.ItemCondition.Value : null,
                            Status = bd.Status
                        }).ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (record is null)
                throw new NotFoundException("Phiếu mượn không tồn tại!");

            if (currentRole == RoleName.READER.ToString() && record.ReaderId != currentUserId)
                throw new ForbiddenException("Bạn không có quyền xem phiếu mượn này!");

            return record;
        }

        // POST /borrow-records
        public async Task<BorrowRecordDetailResponse> CreateBorrowRecordAsync(
            int staffId,
            CreateBorrowRecordRequest request,
            CancellationToken ct = default)
        {
            var reader = await _context.Users
                .Include(u => u.LibraryCard)
                .FirstOrDefaultAsync(u => u.UserId == request.ReaderId, ct);

            if (reader == null)
                throw new NotFoundException("Người dùng không tồn tại!");

            if (reader.Status == UserStatus.LOCKED)
                throw new ForbiddenException("Tài khoản đã bị khóa!");

            if (reader.LibraryCard == null || reader.LibraryCard.Status != CardStatus.ACTIVE)
                throw new ForbiddenException("Thẻ thư viện không hợp lệ!");

            if (reader.LibraryCard.ExpiredAt < DateTime.UtcNow)
                throw new ForbiddenException("Thẻ thư viện đã hết hạn!");

            var hasUnpaidFine = await _context.Fines
                .AnyAsync(f =>
                    f.BorrowDetail.BorrowRecord.ReaderId == request.ReaderId &&
                    f.PaymentStatus == PaymentStatus.PENDING,
                    ct);

            if (hasUnpaidFine)
                throw new BadRequestException("Người dùng còn phiếu phạt chưa thanh toán!");

            if (request.CopyIds.Count > _librarySettings.MaxBooksPerBorrow)
                throw new BadRequestException($"Chỉ được mượn tối đa {_librarySettings.MaxBooksPerBorrow} cuốn!");

            int currentBorrowing = await _context.BorrowDetails
                .Where(bd =>
                    bd.BorrowRecord.ReaderId == request.ReaderId &&
                    bd.BorrowRecord.Status == BorrowStatus.BORROWING &&
                    bd.Status == BorrowDetailStatus.BORROWING)
                .CountAsync(ct);

            if (currentBorrowing + request.CopyIds.Count > _librarySettings.MaxBooksPerBorrow)
                throw new BadRequestException($"Người dùng đang mượn {currentBorrowing} sách, không thể mượn thêm!");

            var copies = await _context.BookCopies
                .Where(bc => request.CopyIds.Contains(bc.CopyId))
                .ToListAsync(ct);

            if (copies.Count != request.CopyIds.Count)
                throw new NotFoundException("Một số bản sao sách không tồn tại!");

            var unavailableCopies = copies.Where(bc => bc.Status != BookCopyStatus.AVAILABLE).ToList();
            if (unavailableCopies.Any())
                throw new BadRequestException($"Một số sách không khả dụng: {string.Join(", ", unavailableCopies.Select(x => x.Barcode))}");

            var borrowRecord = new BorrowRecord
            {
                ReaderId = request.ReaderId,
                ApprovedBy = staffId,
                BorrowCode = GenerateBorrowCode(),
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(_librarySettings.BorrowDurationDays),
                ExtensionCount = 0,
                ExtensionRequestStatus = ExtensionRequestStatus.NONE,
                BorrowType = request.BorrowType,
                Status = BorrowStatus.BORROWING,
                ApprovedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                BorrowDetails = copies.Select(c => new BorrowDetail
                {
                    CopyId = c.CopyId,
                    Status = BorrowDetailStatus.BORROWING
                }).ToList()
            };

            await _context.BorrowRecords.AddAsync(borrowRecord, ct);

            foreach (var copy in copies)
                copy.Status = BookCopyStatus.BORROWED;

            await _context.SaveChangesAsync(ct);

            return await GetBorrowRecordByIdAsync(borrowRecord.BorrowId, staffId, RoleName.STAFF.ToString(), ct);
        }

        // PATCH /borrow-records/:id/return
        public async Task ConfirmReturnAsync(
            int borrowId,
            ConfirmReturnRequest request,
            CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .Include(br => br.BorrowDetails)
                    .ThenInclude(bd => bd.BookCopy)
                        .ThenInclude(bc => bc.Book)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            if (record == null)
                throw new NotFoundException("Phiếu mượn không tồn tại!");

            if (record.Status != BorrowStatus.BORROWING && record.Status != BorrowStatus.OVERDUE)
                throw new BadRequestException("Phiếu mượn không hợp lệ!");

            var returnItems = request.ReturnItems ?? new List<ReturnItemCondition>();

            var validCopyIds = record.BorrowDetails.Select(d => d.CopyId).ToHashSet();
            var invalidCopyIds = returnItems
                .Where(x => !validCopyIds.Contains(x.CopyId))
                .Select(x => x.CopyId)
                .ToList();

            if (invalidCopyIds.Any())
                throw new BadRequestException($"CopyId không hợp lệ hoặc không thuộc phiếu mượn này: {string.Join(", ", invalidCopyIds)}");

            var now = DateTime.UtcNow;

            foreach (var detail in record.BorrowDetails)
            {
                var returnItem = returnItems.FirstOrDefault(x => x.CopyId == detail.CopyId);
                var condition = returnItem?.Condition ?? BookCondition.NORMAL;

                detail.ReturnedAt = now;
                detail.ItemCondition = condition;
                detail.BookCopy.Condition = condition;

                switch (condition)
                {
                    case BookCondition.NORMAL:
                        detail.Status = BorrowDetailStatus.RETURNED;
                        detail.BookCopy.Status = BookCopyStatus.AVAILABLE;
                        break;

                    case BookCondition.TORN:
                    case BookCondition.DAMAGED:
                        detail.Status = BorrowDetailStatus.DAMAGED;
                        detail.BookCopy.Status = BookCopyStatus.UNAVAILABLE;
                        await _context.Fines.AddAsync(new Fine
                        {
                            BorrowDetailId = detail.BorrowDetailId,
                            FineType = FineType.DAMAGED,
                            Amount = returnItem!.FineAmount ?? 0,
                            Reason = returnItem.FineReason ?? "Sách bị hư hỏng",
                            PaymentStatus = PaymentStatus.PENDING,
                            CreatedAt = now
                        }, ct);
                        break;

                    case BookCondition.LOST:
                        detail.Status = BorrowDetailStatus.LOST;
                        detail.BookCopy.Status = BookCopyStatus.UNAVAILABLE;
                        await _context.Fines.AddAsync(new Fine
                        {
                            BorrowDetailId = detail.BorrowDetailId,
                            FineType = FineType.LOST,
                            Amount = returnItem?.FineAmount ?? 0,
                            Reason = returnItem?.FineReason ?? "Sách bị mất",
                            PaymentStatus = PaymentStatus.PENDING,
                            CreatedAt = now
                        }, ct);
                        break;
                }
            }

            // Phạt quá hạn
            if (now > record.DueDate)
            {
                var overdueDays = (now.Date - record.DueDate.Date).Days;
                foreach (var detail in record.BorrowDetails
                    .Where(d => d.Status != BorrowDetailStatus.LOST && d.Status != BorrowDetailStatus.DAMAGED))
                {
                    await _context.Fines.AddAsync(new Fine
                    {
                        BorrowDetailId = detail.BorrowDetailId,
                        FineType = FineType.OVERDUE,
                        Amount = overdueDays * _librarySettings.OverdueFinePerDay,
                        Reason = $"Trả quá hạn {overdueDays} ngày",
                        PaymentStatus = PaymentStatus.PENDING,
                        CreatedAt = now
                    }, ct);
                }
            }

            record.Status = BorrowStatus.RETURNED;
            record.ReturnedDate = now;

            await _context.SaveChangesAsync(ct);
        }

        // PATCH /borrow-records/:id/cancel
        public async Task CancelBorrowRecordAsync(int borrowId, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .Include(br => br.BorrowDetails)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            if (record == null)
                throw new NotFoundException("Phiếu mượn không tồn tại!");

            if (record.Status == BorrowStatus.RETURNED)
                throw new BadRequestException("Phiếu đã hoàn trả không thể hủy!");

            if (record.Status == BorrowStatus.CANCELLED)
                throw new BadRequestException("Phiếu đã bị hủy trước đó!");

            if (record.Status == BorrowStatus.OVERDUE)
                throw new BadRequestException("Phiếu quá hạn không thể hủy!");

            var copyIds = record.BorrowDetails.Select(x => x.CopyId).ToList();
            var copies = await _context.BookCopies
                .Where(x => copyIds.Contains(x.CopyId))
                .ToListAsync(ct);

            foreach (var copy in copies)
                copy.Status = BookCopyStatus.AVAILABLE;

            record.Status = BorrowStatus.CANCELLED;

            await _context.SaveChangesAsync(ct);
        }

        // POST /borrow-records/:id/extension-requests — Reader gửi yêu cầu
        public async Task SubmitExtensionRequestAsync(
            int borrowId,
            int readerId,
            CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId && br.ReaderId == readerId, ct);

            if (record is null)
                throw new NotFoundException("Phiếu mượn không tồn tại!");

            // Chặn gửi lại khi đang có request pending
            if (record.ExtensionRequestStatus == ExtensionRequestStatus.PENDING)
                throw new BadRequestException("Yêu cầu gia hạn đang chờ Staff/Admin xử lý!");

            ValidateExtensionEligibility(record);

            // Chỉ cập nhật trạng thái — không tạo Notification, không gọi SendToStaffAsync
            record.ExtensionRequestStatus = ExtensionRequestStatus.PENDING;

            await _context.SaveChangesAsync(ct);
        }

        // PATCH /borrow-records/:id/extend — Staff duyệt
        public async Task ConfirmExtensionAsync(int borrowId, ProcessExtensionRequest request, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            if (record == null)
                throw new NotFoundException("Phiếu mượn không tồn tại!");

            // Bắt buộc phải có request pending trước
            if (record.ExtensionRequestStatus != ExtensionRequestStatus.PENDING)
                throw new BadRequestException("Không có yêu cầu gia hạn nào đang chờ xử lý!");

            if (request.IsApproved)
            {
                ValidateExtensionEligibility(record);

                record.ExtensionCount += 1;
                record.DueDate = record.DueDate.AddDays(_librarySettings.ExtensionDays);

                // Reset về APPROVED — Reader có thể gửi lại ở lần gia hạn tiếp theo nếu ExtensionCount chưa đạt max
                record.ExtensionRequestStatus = ExtensionRequestStatus.APPROVED;

                await _context.SaveChangesAsync(ct);

                // Gửi thông báo cho Reader
                await _notificationService.SendAsync(
                    record.ReaderId,
                    NotificationType.EXTENSIONAPPROVED,
                    "Gia hạn sách thành công",
                    $"Phiếu mượn {record.BorrowCode} đã được gia hạn. Hạn trả mới: {record.DueDate:dd/MM/yyyy}.",
                    ct);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.Reason))
                    throw new BadRequestException("Vui lòng cung cấp lý do từ chối!");

                record.ExtensionRequestStatus = ExtensionRequestStatus.REJECTED;

                await _context.SaveChangesAsync(ct);

                await _notificationService.SendAsync(
                    record.ReaderId,
                    NotificationType.EXTENSIONREJECTED,
                    "Yêu cầu gia hạn bị từ chối",
                    request.Reason,
                    ct);
            }


        }

        // PRIVATE HELPERS
        private void ValidateExtensionEligibility(BorrowRecord record)
        {
            if (record.Status != BorrowStatus.BORROWING)
                throw new BadRequestException("Phiếu mượn không ở trạng thái đang mượn!");

            if (record.ExtensionCount >= _librarySettings.MaxExtensions)
                throw new BadRequestException($"Đã vượt quá số lần gia hạn tối đa ({_librarySettings.MaxExtensions})!");

            if (DateTime.UtcNow > record.DueDate)
                throw new BadRequestException("Sách đã quá hạn, không thể gia hạn!");
        }

        private static string GenerateBorrowCode()
            => $"BR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").ToUpper()[..6]}";
    }
}