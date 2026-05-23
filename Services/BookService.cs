using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Books;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        public BookService(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<BookResponse> CreateBook(CreateBookRequest request, CancellationToken ct = default)
        {
            // kiem tra xem id cua sach da tao chua
            // 1. Tao roi thi ko cho tao nua
            var isCreated = await _context.Books.AnyAsync(b => b.ISBN == request.ISBN, ct);
            if(isCreated)
            {
                throw new Exception("Book already created");
            }
            // 2. Neu chua tao thi tao sach
            var book = new Book
            {
                Title = request.Title,
                ISBN = request.ISBN,
                CategoryId = request.CategoryId,
                PublisherId = request.PublisherId,
                AuthorId = request.AuthorId,
                Quantity = request.Quantity,
                Language = request.Language,
                Description = request.Description,
                CoverImage = request.CoverImage,
            };

            await _context.Books.AddAsync(book, ct);
            await _context.SaveChangesAsync(ct);

            return new BookResponse(
                book.BookId,
                book.Title,
                book.ISBN,
                book.Quantity,
                book.AvailableQuantity,
                book.Language,
                book.Description,
                book.CoverImage
            );
        }

        public async Task DeleteBook(int BookId, CancellationToken ct = default)
        {
            var deletedRows = await _context.Books
                .Where(b => b.BookId == BookId)
                .ExecuteDeleteAsync(ct);

            if(deletedRows == 0)
            {
                throw new Exception("Book not found!");
            }
        }

        public async Task<PaginationResponse<BookResponse>> GetRangeBooks(BookQueryRequest request, CancellationToken ct = default)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var query = _context.Books.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();
                query = query.Where(b => b.Title.Contains(keyword) || b.ISBN.Contains(keyword));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == request.CategoryId.Value);
            }

            if (request.AuthorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == request.AuthorId.Value);
            }

            // tong so sach
            var total = await query.CountAsync(ct);

            var getRangeBook = await query
                .OrderBy(b => b.BookId)
                .Skip((page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookResponse
                (
                   b.BookId,
                   b.Title,
                   b.ISBN,
                   b.Quantity,
                   b.AvailableQuantity,
                   b.Language,
                   b.Description,
                   b.CoverImage
                )).ToListAsync(ct);

            return new PaginationResponse<BookResponse>
            {
                Items = getRangeBook,
                Page = page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }
        public async Task<bool> UpdateBook(int BookId, UpdateBookRequest request, CancellationToken ct = default)
        {
            var updatedRows = await _context.Books
                .Where(b => b.BookId == BookId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.Title, request.Title)
                    .SetProperty(b => b.ISBN, request.ISBN)
                    .SetProperty(b => b.CategoryId, request.CategoryId)
                    .SetProperty(b => b.AuthorId, request.AuthorId)
                    .SetProperty(b => b.PublisherId, request.PublisherId)
                    .SetProperty(b => b.UpdatedAt, DateTime.UtcNow), ct);

            if(updatedRows == 0)
            {
                throw new Exception("Book Not Found!");
            }

            return true;
        }
        
        public async Task<BookDetailResponse> GetBookById(int BookId, CancellationToken ct = default)
        {
            var book = await _context.Books
                .AsNoTracking()
                .Where(b => b.BookId == BookId)
                .Select(b => new BookDetailResponse(
                    b.BookId,
                    b.Title,
                    b.ISBN,
                    b.Language,
                    b.Description,
                    b.Quantity,
                    b.AvailableQuantity,
                    b.CoverImage,
                    b.CreatedAt,
                    b.UpdatedAt
                ))
                .FirstOrDefaultAsync(ct);

            if(book == null)
            {
                throw new Exception("Book Not Found!");
            }

            return book;
        }
    }
}
