using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Reservations;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class ReservationService : IReservationService
    {
        // Han giu sach sau khi co san: 3 ngay
        private const int ReservationHoldDays = 3;

        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public ReservationService(
            ApplicationDbContext context,
            INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET /reservations — Staff/Admin xem danh sach dat truoc (UC23)
        public async Task<PaginationResponse<ReservationResponse>> GetReservationsAsync(
            int page, int pageSize, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 50 ? 50 : pageSize;

            var query = _context.Reservations
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Book);

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => MapToResponse(r))
                .ToListAsync(ct);

            return new PaginationResponse<ReservationResponse>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }

        // POST /reservations — Staff tao phieu dat truoc (UC23)
        public async Task<ReservationResponse> CreateReservationAsync(
            CreateReservationRequest request, CancellationToken ct = default)
        {
            // Kiem tra sach ton tai
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == request.BookId, ct);

            if (book is null)
                throw new NotFoundException("Sach khong ton tai!");

            // Kiem tra sach hien tai khong con san (moi dat truoc khi het sach)
            if (book.AvailabilityCopies > 0)
                throw new BadRequestException("Sach hien van con ban sao kha dung, vui long muon truc tiep!");

            // B4a: Nguoi dung da co dat truoc chua
            bool alreadyReserved = await _context.Reservations
                .AnyAsync(r =>
                    r.UserId == request.UserId &&
                    r.BookId == request.BookId &&
                    r.Status == ReservationStatus.Waiting, ct);

            if (alreadyReserved)
                throw new ConflictException("Nguoi dung da dat truoc cuon sach nay!");

            var reservation = new Reservation
            {
                UserId = request.UserId,
                BookId = request.BookId,
                ExpiryDate = DateTime.UtcNow.AddDays(ReservationHoldDays),
                Status = ReservationStatus.Waiting,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reservations.AddAsync(reservation, ct);
            await _context.SaveChangesAsync(ct);

            var saved = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Book)
                .FirstAsync(r => r.ReservationId == reservation.ReservationId, ct);

            return MapToResponse(saved);
        }

        // PATCH /reservations/:id/cancel — Huy dat truoc (UC23)
        public async Task CancelReservationAsync(int reservationId, CancellationToken ct = default)
        {
            var reservation = await _context.Reservations.FindAsync(new object[] { reservationId }, ct);

            if (reservation is null)
                throw new NotFoundException("Phieu dat truoc khong ton tai!");

            if (reservation.Status != ReservationStatus.Waiting && reservation.Status != ReservationStatus.Ready)
                throw new BadRequestException("Khong the huy phieu dat truoc o trang thai hien tai!");

            reservation.Status = ReservationStatus.Cancelled;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }

        // PATCH /reservations/:id/complete — Chuyen dat truoc thanh phieu muon (UC23 - B6)
        // Goi khi sach da co san, Staff xac nhan chuyen sang muon
        public async Task<int> CompleteReservationAsync(int reservationId, int staffId, CancellationToken ct = default)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId, ct);

            if (reservation is null)
                throw new NotFoundException("Phieu dat truoc khong ton tai!");

            if (reservation.Status != ReservationStatus.Ready)
                throw new BadRequestException("Phieu dat truoc chua o trang thai san sang de chuyen thanh phieu muon!");

            // Tim ban sao kha dung de gan vao phieu muon
            var availableCopy = await _context.BookCopies
                .FirstOrDefaultAsync(bc =>
                    bc.BookId == reservation.BookId &&
                    bc.Status == BookCopyStatus.Available &&
                    !bc.IsReferenceOnly, ct);

            if (availableCopy is null)
                throw new BadRequestException("Khong con ban sao kha dung de tao phieu muon!");

            // Tao phieu muon tu phieu dat truoc
            var borrowRecord = new BorrowRecord
            {
                ReaderId = reservation.UserId,
                ApprovedBy = staffId,
                BorrowCode = $"BR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").ToUpper()[..6]}",
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                ExtensionCount = 0,
                BorrowType = BorrowType.TakeHome,
                Status = BorrowStatus.Borrowing,
                ApprovedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                BorrowDetails = new List<BorrowDetail>
                {
                    new() { CopyId = availableCopy.CopyId, Status = BorrowDetailStatus.Borrowing }
                }
            };

            availableCopy.Status = BookCopyStatus.Borrowed;

            await _context.Books
                .Where(b => b.BookId == reservation.BookId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(b => b.AvailabilityCopies, b => b.AvailabilityCopies - 1), ct);

            reservation.Status = ReservationStatus.Completed;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.BorrowRecords.AddAsync(borrowRecord, ct);
            await _context.SaveChangesAsync(ct);

            await _notificationService.SendAsync(
                reservation.UserId,
                "Dat truoc sach thanh cong",
                $"Sach '{reservation.Book.Title}' da san sang. Phieu muon {borrowRecord.BorrowCode} da duoc tao.", ct);

            return borrowRecord.BorrowId;
        }

        // Private helper
        private static ReservationResponse MapToResponse(Reservation r) => new()
        {
            ReservationId = r.ReservationId,
            UserId = r.UserId,
            UserName = r.User?.FullName ?? string.Empty,
            BookId = r.BookId,
            BookTitle = r.Book?.Title ?? string.Empty,
            ExpiryDate = r.ExpiryDate,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}