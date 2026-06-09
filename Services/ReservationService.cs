using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Reservations;
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
    public class ReservationService : IReservationService
    {
        private readonly LibrarySettings _librarySettings;
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        public ReservationService(
            ApplicationDbContext context,
            INotificationService notificationService,
            IOptions<LibrarySettings> librarySettings)
        {
            _context = context;
            _notificationService = notificationService;
            _librarySettings = librarySettings.Value;
        }

        // GET /reservations — Staff/Admin xem danh sach dat truoc
        public async Task<PaginationResponse<ReservationResponse>> GetReservationsAsync(
            PaginationRequest request, CancellationToken ct = default)
        {
            var query = _context.Reservations.AsNoTracking();
            int total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new ReservationResponse(
                    r.ReservationId,
                    r.UserId,
                    r.User.FullName,
                    r.BookId,
                    r.Book.Title,
                    r.Status.ToString(),
                    r.CreatedAt
                )).ToListAsync(ct);
            return new PaginationResponse<ReservationResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }
        // POST /reservations — Staff tao phieu dat truoc
        public async Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request, CancellationToken ct = default)
        {
            // TH1: Kiem tra nguoi dung
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.LibraryCard)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, ct);
            if (user == null)
                throw new NotFoundException("Nguoi dung khong ton tai!");
            // Kiem tra tai khoan bi khoa
            if (user.Status != UserStatus.ACTIVE)
                throw new BadRequestException("Tai khoan nguoi dung dang bi khoa hoac tam ngung!");
            // Kiem tra the thu vien
            if (user.LibraryCard == null)
                throw new BadRequestException("Nguoi dung chua duoc cap the thu vien!");
            if (user.LibraryCard.Status != CardStatus.ACTIVE)
                throw new BadRequestException("The thu vien da bi khoa hoac het han!");
            if (user.LibraryCard.ExpiredAt < DateTime.UtcNow)
                throw new BadRequestException("The thu vien da het han!");
            // --- TH2: Kiem tra phieu phat chua thanh toan ---
            // Neu nguoi dung con phieu phat PENDING thi khong cho dat truoc
            bool hasUnpaidFine = await _context.Fines
                .AsNoTracking()
                .AnyAsync(f =>
                    f.BorrowDetail.BorrowRecord.ReaderId == request.UserId &&
                    f.PaymentStatus == PaymentStatus.PENDING,
                ct);
            if (hasUnpaidFine)
                throw new BadRequestException("Nguoi dung con phieu phat chua thanh toan, khong the dat truoc sach!");
            // TH3: Kiem tra dau sach
            bool bookExists = await _context.Books.AsNoTracking().AnyAsync(b => b.BookId == request.BookId, ct);
            if (!bookExists)
                throw new NotFoundException("Sach khong ton tai!");
            // Chi cho dat truoc khi KHONG con ban sao nao kha dung
            bool hasAvailableCopy = await _context.BookCopies
                .AsNoTracking()
                .AnyAsync(bc =>
                    bc.BookId == request.BookId &&
                    bc.Status == BookCopyStatus.AVAILABLE,
                ct);

            if (hasAvailableCopy)
                throw new BadRequestException("Sach van con ban sao kha dung, vui long muon truc tiep!");

            // --- TH4: Kiem tra da dat truoc chua ---
            bool alreadyReserved = await _context.Reservations.AsNoTracking()
                .AnyAsync(r =>
                    r.UserId == request.UserId &&
                    r.BookId == request.BookId &&
                    r.Status == ReservationStatus.WAITING, ct);
            if (alreadyReserved)
                throw new ConflictException("Nguoi dung da dat truoc cuon sach nay!");
            // --- Tao phieu dat truoc ---
            var reservation = new Reservation
            {
                UserId = request.UserId,
                BookId = request.BookId,
                Status = ReservationStatus.WAITING,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Reservations.AddAsync(reservation, ct);
            await _context.SaveChangesAsync(ct);
            // Tra ve response voi full thong tin bang 1 query
            return await _context.Reservations
                .AsNoTracking()
                .Where(r => r.ReservationId == reservation.ReservationId)
                .Select(r => new ReservationResponse(
                    r.ReservationId,
                    r.UserId,
                    r.User.FullName,
                    r.BookId,
                    r.Book.Title,
                    r.Status.ToString(),
                    r.CreatedAt
                )).FirstAsync(ct);
        }
        // PATCH /reservations/:id/cancel — Huy dat truoc   ADMIN/STAFF
        public async Task CancelReservationAsync(int reservationId, CancellationToken ct = default)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.ReservationId == reservationId, ct);
            if(reservation == null)
            {
                throw new NotFoundException("Phieu dat truoc khong ton tai!");
            }
            if(reservation.Status != ReservationStatus.WAITING)
            {
                throw new BadRequestException("Chi duoc huy phieu dat truoc dang cho!");
            }
            reservation.Status = ReservationStatus.CANCELLED;
            reservation.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
        // PATCH /reservations/:id/complete — Chuyen dat truoc thanh phieu muon
        // Goi khi sach da co san, Staff xac nhan chuyen sang muon
        public async Task<int> CompleteReservationAsync(int reservationId, int staffId, CancellationToken ct = default)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Book)
                    .ThenInclude(b => b.BookCopies)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId, ct);
            if(reservation == null)
            {
                throw new NotFoundException("Phieu dat truoc khong ton tai!");
            }
            /***
             * WAITING => dang cho => co the complete hoac cancel
             * COMPLETED => da chuyen thanh phieu muon khong the thao tac
             * CANCELLED => da huy khong the thao tac
             * EXPIRED => het han khong the thao tac
             */
            if(reservation.Status != ReservationStatus.WAITING)
            {
                throw new BadRequestException("chi co the chuyen phieu dang o trang thai waiting thanh phieu muon!");
            }
            // Tim ban sao Available cua dau sach nay
            var availableCopy = reservation.Book.BookCopies.FirstOrDefault(bc => bc.Status == BookCopyStatus.AVAILABLE);
            if(availableCopy == null)
            {
                throw new BadRequestException("Hien tai khong co ban sao kha dung de tao phieu muon!");
            }
            // Kiem tra nguoi dung con hop le
            var user = await _context.Users.AsNoTracking().Include(u => u.LibraryCard).FirstOrDefaultAsync(u => u.UserId == reservation.UserId, ct);
            if(user == null)
            {
                throw new NotFoundException("Nguoi dung khong ton tai!");
            }
            if(user.Status != UserStatus.ACTIVE)
            {
                throw new BadRequestException("The thu vien khong hop le hoac bi khoa!");
            }
            if(user.LibraryCard.ExpiredAt < DateTime.UtcNow)
            {
                throw new BadRequestException("The thu vien da het han!");
            }
            // Tao phieu muon
            var borrowRecord = new BorrowRecord
            {
                ReaderId = reservation.UserId,
                ApprovedBy = staffId,
                BorrowCode = GenerateBorrowCode(),
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(_librarySettings.BorrowDurationDays),
                ExtensionCount = 0,
                BorrowType = BorrowType.TAKEHOME,
                Status = BorrowStatus.BORROWING,
                ApprovedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                BorrowDetails = new List<BorrowDetail>
                {
                    new BorrowDetail
                    {
                        CopyId = availableCopy.CopyId,
                        Status = BorrowDetailStatus.BORROWING,
                    }
                }
            };
            // Cap nhat trang thai ban sao: available => borrowed
            availableCopy.Status = BookCopyStatus.BORROWED;
            // Cap nhat trang thai phieu dat truong WAITING => COMPLETED
            reservation.Status = ReservationStatus.COMPLETED;
            reservation.UpdatedAt = DateTime.UtcNow;
            await _context.BorrowRecords.AddAsync(borrowRecord, ct);
            await _context.SaveChangesAsync(ct);
            await _notificationService.SendAsync(
                reservation.UserId,
                NotificationType.SYSTEMANNOUNCEMENT,
                "Đặt trước sách thành công",
                $"Sách {reservation.Book.Title} đã được chuyển thành phiếu mượn {borrowRecord.BorrowCode}. Hạn trả: {borrowRecord.DueDate:dd/MM/yyyy}.",
                ct);
            return borrowRecord.BorrowId; // tra ve BorrowId 
        }
        // private function helper
        private static string GenerateBorrowCode()
        {
            return $"BR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").ToUpper()[..6]}";
        }
    }
}