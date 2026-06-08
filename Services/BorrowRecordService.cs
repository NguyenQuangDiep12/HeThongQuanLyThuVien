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
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Services
{
    public class BorrowRecordService : IBorrowRecordService
    {

        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly LibrarySettings _librarySettings;

        public BorrowRecordService(
            ApplicationDbContext context,
            INotificationService notificationService,
            IHttpContextAccessor contextAccessor,
            IOptions<LibrarySettings> librarySettings)
        {
            _context = context;
            _notificationService = notificationService;
            _contextAccessor = contextAccessor;
            _librarySettings = librarySettings.Value;
        }

        // GET /borrow-records | GET | /borrow-records | Danh sách phiếu mượn | Staff/Admin |
        public async Task<PaginationResponse<BorrowRecordSummaryResponse>>GetBorrowRecordsAsync(BorrowRecordQueryRequest request, CancellationToken ct = default)
        {
            IQueryable<BorrowRecord> query = _context.BorrowRecords.AsNoTracking();
            // Filter theo mã phiếu
            if (!string.IsNullOrWhiteSpace(request.BorrowCode))
            {
                query = query.Where(br => br.BorrowCode.Contains(request.BorrowCode));
            }
            // Filter theo tên người đọc
            if (!string.IsNullOrWhiteSpace(request.ReaderName))
            {
                query = query.Where(br => br.Reader.FullName.Contains(request.ReaderName));
            }
            // Filter theo trạng thái
            if (request.Status.HasValue)
            {
                query = query.Where(br => br.Status == request.Status.Value);
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
                    TotalBooks = br.BorrowDetails.Count()
                }).ToListAsync(ct);
            return new PaginationResponse<BorrowRecordSummaryResponse>
            {
                Items = items,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        // GET /users/:id/borrow-records | GET | /users/:id/borrow-records | Lịch sử mượn sách của người dùng | Owner/Staff/Admin |
        public async Task<PaginationResponse<BorrowRecordSummaryResponse>> GetUserBorrowRecordsAsync(int userId, PaginationRequest request, CancellationToken ct = default)
        {
            int currentUserId = int.Parse(_contextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string currentRole = _contextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value;
            IQueryable<BorrowRecord> query = _context.BorrowRecords.AsNoTracking();
            // Reader chỉ xem của chính mình
            if (currentRole == RoleName.READER.ToString())
            {
                if (currentUserId != userId)
                {
                    throw new ForbiddenException("Bạn không có quyền xem lịch sử mượn của người khác!");
                }
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
                    TotalBooks = br.BorrowDetails.Count()
                }).ToListAsync(ct);
            return new PaginationResponse<BorrowRecordSummaryResponse>
            {
                Items = items,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        // GET /borrow-records/:id | GET | /borrow-records/:id | Chi tiết phiếu mượn | Staff/Admin |
        public async Task<BorrowRecordDetailResponse> GetBorrowRecordByIdAsync(int borrowId, CancellationToken ct = default)
        {

            var record = await _context.BorrowRecords.AsNoTracking().Where(br => br.BorrowId == borrowId)
                .Select(br => new BorrowRecordDetailResponse
                {
                    BorrowId = br.BorrowId,
                    BorrowCode = br.BorrowCode,
                    ReaderId = br.ReaderId,
                    ReaderName = br.Reader.FullName,
                    ApproverId = br.ApprovedBy,
                    ApproverName = br.Approver != null
                        ? br.Approver.FullName
                        : null,
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
                            ItemCondition = bd.ItemCondition.HasValue ? bd.ItemCondition.Value: null,
                            Status = bd.Status
                        }).ToList()
                }).FirstOrDefaultAsync(ct);

            if (record is null)
            {
                throw new NotFoundException("Phiếu mượn không tồn tại!");
            }

            return record;
        }
        // POST /borrow-records | POST | /borrow-records | Tạo phiếu mượn mới | Staff/Admin |
        public async Task<BorrowRecordDetailResponse> CreateBorrowRecordAsync(int staffId, CreateBorrowRecordRequest request, CancellationToken ct = default)
        {
            // Kiểm tra người dùng
            var reader = await _context.Users.Include(u => u.LibraryCard).FirstOrDefaultAsync(u => u.UserId == request.ReaderId, ct);
            if (reader == null)
            {
                throw new NotFoundException("Người dùng không tồn tại!");
            }
            // Kiểm tra trạng thái tài khoản
            if (reader.Status == UserStatus.LOCKED)
            {
                throw new ForbiddenException("Tài khoản đã bị khóa!");
            }
            // Kiểm tra thẻ thư viện
            if (reader.LibraryCard == null || reader.LibraryCard.Status != CardStatus.ACTIVE)
            {
                throw new ForbiddenException("Thẻ thư viện không hợp lệ!");
            }
            // Kiểm tra hạn thẻ
            if (reader.LibraryCard.ExpiredAt < DateTime.UtcNow)
            {
                throw new ForbiddenException("Thẻ thư viện đã hết hạn!");
            }

            bool hasUnpaidFine = await _context.Fines.AnyAsync(f =>
                f.BorrowDetail.BorrowRecord.ReaderId == request.ReaderId &&
                f.PaymentStatus == PaymentStatus.PENDING, ct);

            if (hasUnpaidFine)
            {
                throw new ForbiddenException("Độc giả còn phiếu phạt chưa thanh toán!");
            }

            // Kiểm tra số lượng sách mượn
            if (request.CopyIds.Count > _librarySettings.MaxBooksPerBorrow)
            {
                throw new BadRequestException($"Chỉ được mượn tối đa {_librarySettings.MaxBooksPerBorrow} cuốn!");
            }

            // Đếm số sách đang mượn
            int currentBorrowing = await _context.BorrowDetails.Where(bd =>
                    bd.BorrowRecord.ReaderId == request.ReaderId &&
                    bd.BorrowRecord.Status == BorrowStatus.BORROWING &&
                    bd.Status == BorrowDetailStatus.BORROWING).CountAsync(ct);

            // Kiểm tra số sách đang mượn trước đó và số sách hiện tại mượn vượt quá số sách được cho phép mượn
            // ném lỗi người dùng vượt quá số sách
            if (currentBorrowing + request.CopyIds.Count > _librarySettings.MaxBooksPerBorrow)
            {
                throw new BadRequestException($"Người dùng đang mượn {currentBorrowing} sách!");
            }
            // Lấy danh sách bản sao
            var copies = await _context.BookCopies.Where(bc => request.CopyIds.Contains(bc.CopyId)).ToListAsync(ct);
            // Gia su CopyIds = [2,3,4]
            // SELECT * from BookCopies where CopyId In [2,3,4]
            // Kiểm tra tồn tại
            if (copies.Count != request.CopyIds.Count)
            {
                throw new NotFoundException("Một số bản sao sách không tồn tại!");
            }
            // Kiểm tra khả dụng
            var unavailableCopies = copies.Where(bc => bc.Status != BookCopyStatus.AVAILABLE).ToList();
            if (unavailableCopies.Any())
            {
                throw new BadRequestException($"Một số sách không khả dụng: " + $"{string.Join(", ",unavailableCopies.Select(x => x.Barcode))}");
            }
            // Tạo phiếu mượn
            var borrowRecord = new BorrowRecord
            {
                ReaderId = request.ReaderId,
                ApprovedBy = staffId,
                BorrowCode = GenerateBorrowCode(),
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(_librarySettings.BorrowDurationDays),
                ExtensionCount = 0,
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
            // Cập nhật trạng thái bản sao
            foreach (var copy in copies)
            {
                copy.Status = BookCopyStatus.BORROWED;
            }
            await _context.SaveChangesAsync(ct);
            return await GetBorrowRecordByIdAsync(borrowRecord.BorrowId, ct);
        }
        // PATCH /borrow-records/:id/return | PATCH | /borrow-records/:id/return | Xác nhận trả sách | Staff/Admin |
        public async Task ConfirmReturnAsync(int borrowId, ConfirmReturnRequest request, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .Include(br => br.BorrowDetails)
                    .ThenInclude(bd => bd.BookCopy)
                        .ThenInclude(bc => bc.Book)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);
            if (record == null)
            {
                throw new NotFoundException("Phiếu mượn không tồn tại!");
            }
            // Chỉ xử lý khi đang mượn hoặc quá hạn
            if (record.Status != BorrowStatus.BORROWING && record.Status != BorrowStatus.OVERDUE)
            {
                throw new BadRequestException("Phiếu mượn không hợp lệ!");
            }
            var returnItems = request.ReturnItems ?? new List<ReturnItemCondition>();


            // Validate: Cac copyId trong request phai thuoc ve BorrowRecord nay
            var validCopyIds = record.BorrowDetails.Select(d => d.CopyId).ToHashSet();
            var invalidCopyIds = returnItems

                .Where(x => !validCopyIds.Contains(x.CopyId))
                .Select(x => x.CopyId)
                .ToList();
            if (invalidCopyIds.Any())
            {
                throw new BadRequestException($"CopyId không hợp lệ hoặc không thuộc phiếu mượn này: {string.Join(", ", invalidCopyIds)}");
            }
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
                        await _context.Fines.AddAsync(
                            new Fine
                            {
                                BorrowDetailId = detail.BorrowDetailId,
                                FineType = FineType.DAMAGED,
                                Amount = returnItem?.FineAmount ?? 0,
                                Reason = returnItem?.FineReason ?? "Sách bị hư hỏng",
                                PaymentStatus = PaymentStatus.PENDING,
                                CreatedAt = now
                            }, ct);
                        break;
                    case BookCondition.LOST:
                        detail.Status = BorrowDetailStatus.LOST;
                        detail.BookCopy.Status = BookCopyStatus.UNAVAILABLE;
                        await _context.Fines.AddAsync(
                            new Fine
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

            var overDueDays = (now.Date - record.DueDate.Date).Days;
            // Phạt quá hạn
            if (now > record.DueDate)
            {
                foreach (var detail in record.BorrowDetails)
                {
                    await _context.Fines.AddAsync(
                        new Fine
                        {
                            BorrowDetailId = detail.BorrowDetailId,
                            FineType = FineType.OVERDUE,
                            Amount = overDueDays * 5000,
                            Reason = $"Trả quá hạn {overDueDays} ngày",
                            PaymentStatus = PaymentStatus.PENDING,
                            CreatedAt = now
                        }, ct);
                }
            }
            record.Status = BorrowStatus.RETURNED;
            record.ReturnedDate = now;
            await _context.SaveChangesAsync(ct);
        }
        // PATCH /borrow-records/:id/cancel | Hủy phiếu mượn | Staff/Admin
        public async Task CancelBorrowRecordAsync(int borrowId, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords
                .Include(br => br.BorrowDetails)
                    .ThenInclude(bd => bd.BookCopy)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);

            if (record == null) throw new NotFoundException("Phiếu mượn không tồn tại!");

            // Chỉ hủy được khi đang BORROWING (nhập nhầm) — không cho hủy RETURNED/OVERDUE
            if (record.Status != BorrowStatus.BORROWING) throw new BadRequestException("Chỉ có thể hủy phiếu đang mượn!");


            // Hoàn trả trạng thái các bản sao về AVAILABLE
            foreach (var detail in record.BorrowDetails)
            {
                detail.BookCopy.Status = BookCopyStatus.AVAILABLE;
            }

            record.Status = BorrowStatus.CANCELLED;
            await _context.SaveChangesAsync(ct);
        }

        // POST /borrow-records/:id/extension-requests | POST | /borrow-records/:id/extension-requests | Gửi yêu cầu gia hạn sách | Owner |
        public async Task SubmitExtensionRequestAsync(int borrowId, int readerId, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords.FirstOrDefaultAsync(br => br.BorrowId == borrowId && br.ReaderId == readerId, ct);
            if (record is null)
            {
                throw new NotFoundException("Phiếu mượn không tồn tại!");
            }
            ValidateExtensionEligibility(record);
            await _notificationService.SendToStaffAsync($"Yêu cầu gia hạn phiếu mượn ", $"{record.BorrowCode} từ người dùng {readerId}.", ct);
        }

        // PATCH /borrow-records/:id/extend | PATCH | /borrow-records/:id/extend | Gia hạn sách | Staff/Admin |
        public async Task ConfirmExtensionAsync(int borrowId, int staffId, CancellationToken ct = default)
        {
            var record = await _context.BorrowRecords.FirstOrDefaultAsync(br => br.BorrowId == borrowId, ct);
            if (record is null)
            {
                throw new NotFoundException("Phiếu mượn không tồn tại!");
            }
            ValidateExtensionEligibility(record);
            record.ExtensionCount += 1;
            record.DueDate = record.DueDate.AddDays(_librarySettings.ExtensionDays);
            await _context.SaveChangesAsync(ct);
            // Gửi thông báo
            await _notificationService.SendAsync(record.ReaderId,"Gia hạn sách thành công",$"Phiếu mượn {record.BorrowCode} " + $"đã được gia hạn. " + $"Hạn trả mới: {record.DueDate:dd/MM/yyyy}.", ct);
        }

        // PRIVATE HELPERS
        private void ValidateExtensionEligibility(BorrowRecord record)
        {
            if (record.Status != BorrowStatus.BORROWING)
            {
                throw new BadRequestException("Phiếu mượn không ở trạng thái đang mượn!");
            }
            if (record.ExtensionCount >= _librarySettings.MaxExtensions)
            {
                throw new BadRequestException($"Đã vượt quá số lần gia hạn tối đa ({_librarySettings.MaxExtensions})!");
            }
            if (DateTime.UtcNow > record.DueDate)
            {
                throw new BadRequestException("Sách đã quá hạn, không thể gia hạn!");
            }
        }
        private static string GenerateBorrowCode()
        {
            return $"BR-{DateTime.UtcNow:yyyyMMdd}-" + $"{Guid.NewGuid().ToString("N").ToUpper()[..6]}";
        }
    }
}