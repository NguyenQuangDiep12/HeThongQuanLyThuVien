using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.BookCopies;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Claims;

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

        // GET /book-copies  STAFF/ADMIN
        public async Task<PaginationResponse<BookCopyResponse>> GetBookCopiesAsync(GetBookCopiesRequest request, CancellationToken ct = default)
        {
            var query = _context.BookCopies
                .AsNoTracking()
                .Include(bc => bc.Book) // include de lay book title
                .AsQueryable();

            // 1. loc theo dieu kien dong BookId 
            if (request.BookId.HasValue)
            {
                query = query.Where(bc => bc.BookId == request.BookId.Value);
            }

            // 2. loc theo Trang thai BookCopies:  Available, Borrowed, Reserved, Lost, Damaged
            if (request.Status.HasValue)
            {
                query = query.Where(bc => bc.Status == request.Status.Value);
            }

            // 3. dem tong so ban ghi thoa man dieu kien loc
            int total = await query.CountAsync(ct);

            // 4. phan trang va select du lieu de toi uu hieu nang sang DTO response

            var items = await query
                .OrderBy(bc => bc.CopyId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(bc => new BookCopyResponse
                {
                    CopyId = bc.CopyId,
                    Barcode = bc.Barcode,
                    BookId = bc.BookId,
                    BookTitle = bc.Book != null ? bc.Book.Title : string.Empty,
                    ShelfLocation = bc.ShelfLocation,
                    Condition = bc.Condition.ToString(),
                    Status = bc.Status.ToString(),
                    CreatedAt = bc.CreatedAt,
                }).ToListAsync(ct);

            // 5. Tra ve object ket qua boc trong PaginationResponse 
            return new PaginationResponse<BookCopyResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total,
            };
        }
        // GET | /book-copies/:id     STAFF/ADMIN
        public async Task<BookCopyDetailResponse> GetBookCopyByIdAsync(int copyId, CancellationToken ct = default)
        {
            var Data = await _context.BookCopies
                .AsNoTracking()
                .Where(bc => bc.CopyId == copyId)
                .Select(bc => new BookCopyDetailResponse
                {
                    CopyId = bc.CopyId,
                    Barcode = bc.Barcode,
                    ShelfLocation = bc.ShelfLocation,
                    Condition = bc.Condition.ToString(),
                    Status = bc.Status.ToString(),
                    CreatedAt = bc.CreatedAt,
                    BookId = bc.BookId,
                    BookTitle = bc.Book != null ? bc.Book.Title : string.Empty,
                    Isbn = bc.Book != null ? bc.Book.ISBN : string.Empty,

                    // Lay ra danh sach chuoi ten tac gia (EF dich thanh SubQuery)
                    AuthorName = bc.Book != null
                        ? bc.Book.BookAuthors.Select(ba => ba.Author.AuthorName).ToList() : new List<string>(),

                    Publisher = bc.Book != null && bc.Book.Publisher != null
                        ? bc.Book.Publisher.PublisherName : null
                })
                .FirstOrDefaultAsync(ct);

            if (Data == null)
            {
                throw new NotFoundException("Ban sao sach khong ton tai!");
            }

            return Data;
        }
        // | POST | /book-copies/book/:id | Thêm bản sao sách | Staff/Admin |
        public async Task<BookCopyResponse> CreateBookCopyAsync(int bookId, CreateBookCopyRequest request, CancellationToken ct = default)
        {
            // Kiểm tra đầu sách tồn tại
            bool bookExists = await _context.Books
                .AnyAsync(b => b.BookId == bookId, ct);

            if (!bookExists)
                throw new NotFoundException("Đầu sách không tồn tại!");

            // Kiểm tra barcode trùng
            bool barcodeExists = await _context.BookCopies
                .AnyAsync(bc => bc.Barcode == request.Barcode, ct);

            if (barcodeExists)
                throw new ConflictException("Barcode đã tồn tại!");

            // Tạo bản sao sách
            var copy = new BookCopy
            {
                BookId = bookId,
                Barcode = request.Barcode,
                ShelfLocation = request.ShelfLocation,
                Status = BookCopyStatus.AVAILABLE,
                Condition = BookCondition.NORMAL,
                CreatedAt = DateTime.UtcNow
            };

            await _context.BookCopies.AddAsync(copy, ct);
            await _context.SaveChangesAsync(ct);

            // Load tên sách
            string bookTitle = await _context.Books
                .Where(b => b.BookId == bookId)
                .Select(b => b.Title)
                .FirstAsync(ct);

            return new BookCopyResponse
            {
                CopyId = copy.CopyId,
                BookId = copy.BookId,
                Barcode = copy.Barcode,
                ShelfLocation = copy.ShelfLocation ?? string.Empty,
                Status = copy.Status.ToString(),
                Condition = copy.Condition.ToString(),
                CreatedAt = copy.CreatedAt,
                BookTitle = bookTitle
            };
        }
        // | PUT | /book-copies/:id | Cập nhật tình trạng bản sao | Staff/Admin |
        public async Task UpdateBookCopyAsync(int copyId, UpdateBookCopyRequest request, CancellationToken ct = default)
        {
            var copy = await _context.BookCopies
                .FirstOrDefaultAsync(bc => bc.CopyId == copyId, ct);

            if (copy is null)
                throw new NotFoundException("Bản sao sách không tồn tại!");

            if (request.ShelfLocation is not null)
                copy.ShelfLocation = request.ShelfLocation;

            if (request.Condition.HasValue)
                copy.Condition = request.Condition.Value;

            await _context.SaveChangesAsync(ct);
        }
        // | PATCH | /book-copies/:id/status | Thay đổi trạng thái bản sao | Staff/Admin |
        public async Task ChangeBookCopyStatusAsync(int copyId, UpdateBookCopyStatusRequest request, CancellationToken ct = default)
        {
            int rows = await _context.BookCopies
                .Where(bc => bc.CopyId == copyId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(bc => bc.Status, request.Status),
                    ct);

            if (rows == 0)
                throw new NotFoundException("Bản sao sách không tồn tại!");
        }
        // | DELETE | /book-copies/:id | Xóa bản sao sách | Admin |
        public async Task DeleteBookCopyAsync(int copyId, CancellationToken ct = default)
        {
            // Kiểm tra quyền Admin
            var role = _contextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.Role)?
                .Value;

            if (role != "ADMIN")
                throw new ForbiddenException(
                    "Chỉ Admin mới có quyền xóa bản sao sách!");

            var copy = await _context.BookCopies
                .FirstOrDefaultAsync(bc => bc.CopyId == copyId, ct);

            if (copy is null)
                throw new NotFoundException("Bản sao sách không tồn tại!");

            // Không cho xóa nếu đang được mượn
            if (copy.Status == BookCopyStatus.BORROWED)
                throw new BadRequestException(
                    "Không thể xóa bản sao đang được mượn!");

            _context.BookCopies.Remove(copy);

            await _context.SaveChangesAsync(ct);
        }
    }
}