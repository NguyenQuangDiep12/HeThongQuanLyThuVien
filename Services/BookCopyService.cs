using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.BookCopies;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class BookCopyService : IBookCopyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public BookCopyService(
            ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        // GET /book-copies — Staff/Admin xem danh sach ban sao
        public async Task<List<BookCopyResponse>> GetBookCopiesAsync(int? bookId, CancellationToken ct = default)
        {
            var query = _context.BookCopies.AsNoTracking();

            if (bookId.HasValue)
            {
                query = query.Where(bc => bc.BookId == bookId);
            }

            return await query
                .Select(bc => new BookCopyResponse
                {
                    CopyId = bc.CopyId,
                    BookId = bc.BookId,
                    BookTitle = bc.Book != null ? bc.Book.Title : string.Empty,
                    Barcode = bc.Barcode,
                    ShelfLocation = bc.ShelfLocation,
                    Status = bc.Status.ToString(),
                    IsReferenceOnly = bc.IsReferenceOnly,
                    CreatedAt = bc.CreatedAt,
                })
                .ToListAsync(ct);
        }

        // GET /book-copies/:id — Staff/Admin xem chi tiet ban sao
        public async Task<BookCopyResponse> GetBookCopyByIdAsync(int copyId, CancellationToken ct = default)
        {
            var BookCopy = await _context.BookCopies
                 .AsNoTracking()
                 .Where(bc => bc.CopyId == copyId)
                 .Select(bc => new BookCopyResponse
                 {
                     CopyId = bc.CopyId,
                     BookId = bc.BookId,
                     BookTitle = bc.Book != null ? bc.Book.Title : string.Empty,
                     Barcode = bc.Barcode,
                     ShelfLocation = bc.ShelfLocation,
                     Status = bc.Status.ToString(),
                     Condition = bc.Condition.ToString(),
                     IsReferenceOnly = bc.IsReferenceOnly,
                     CreatedAt = bc.CreatedAt
                 })
                 .FirstOrDefaultAsync(ct);

            if(BookCopy == null)
            {
                throw new NotFoundException("Ban sao sach khong ton tai!");
            }

            return BookCopy;
        }

        // POST /book-copies/book/:id — Staff/Admin them ban sao cho mot dau sach
        public async Task<BookCopyResponse> CreateBookCopyAsync(int bookId, CreateBookCopyRequest request, CancellationToken ct = default)
        {
            bool bookExists = await _context.Books.AnyAsync(b => b.BookId == bookId, ct);
            if (!bookExists)
                throw new NotFoundException("Dau sach khong ton tai!");

            bool barcodeExists = await _context.BookCopies.AnyAsync(bc => bc.Barcode == request.Barcode, ct);
            if (barcodeExists)
                throw new ConflictException("Ma barcode nay da ton tai!");

            var copy = new BookCopy
            {
                BookId = bookId,
                Barcode = request.Barcode,
                ShelfLocation = request.ShelfLocation,
                Status = BookCopyStatus.Available,
                Condition = BookCondition.Normal,
                IsReferenceOnly = request.IsReferenceOnly,
                CreatedAt = DateTime.UtcNow
            };


        }

        // PUT /book-copies/:id — Staff/Admin cap nhat toan bo thong tin ban sao (UC17)
        public async Task<BookCopyResponse> UpdateBookCopyAsync(int copyId, UpdateBookCopyRequest request, CancellationToken ct = default)
        {
            var copy = await _context.BookCopies
                .Include(bc => bc.Book)
                .FirstOrDefaultAsync(bc => bc.CopyId == copyId, ct);

            if (copy is null)
                throw new NotFoundException("Ban sao sach khong ton tai!");

            copy.ShelfLocation = request.ShelfLocation;
            copy.Condition = request.Condition;
            copy.IsReferenceOnly = request.IsReferenceOnly;

            await _context.SaveChangesAsync(ct);

            return MapToResponse(copy);
        }

        // PATCH /book-copies/:id/status — Staff/Admin doi trang thai ban sao
        public async Task ChangeBookCopyStatusAsync(int copyId, UpdateBookCopyStatusRequest request, CancellationToken ct = default)
        {
            var currentBookCopy = await _context.BookCopies
                .FirstOrDefaultAsync(bc => bc.CopyId == copyId, ct);

            if(currentBookCopy == null)
            {
                throw new NotFoundException("Ban sao sach khong ton tai!");
            }

            var oldStatus = currentBookCopy.Status;

            // neu khong thay doi trang thai giu nguyen trang thai hien tai
            if(oldStatus == request.Status)
            {
                return;
            }

            currentBookCopy.Status = request.Status;


        }

        // DELETE /book-copies/:id — chi Admin duoc xoa
        public async Task DeleteBookCopyAsync(int copyId, CancellationToken ct = default)
        {
            var copy = await _context.BookCopies
                .Include(bc => bc.BorrowDetails)
                .FirstOrDefaultAsync(bc => bc.CopyId == copyId, ct);

            if (copy is null)
                throw new NotFoundException("Ban sao sach khong ton tai!");

            // Khong cho xoa neu dang duoc muon
            bool isBorrowed = copy.BorrowDetails.Any(bd => bd.Status == BorrowDetailStatus.Borrowing);
            if (isBorrowed)
                throw new BadRequestException("Khong the xoa ban sao dang duoc muon!");

            int bookId = copy.BookId;
            bool wasAvailable = copy.Status == BookCopyStatus.Available;

            _context.BookCopies.Remove(copy);

            if (wasAvailable)
            {
                await _context.Books
                    .Where(b => b.BookId == bookId)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(b => b.AvailabilityCopies, b => b.AvailabilityCopies - 1), ct);
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}