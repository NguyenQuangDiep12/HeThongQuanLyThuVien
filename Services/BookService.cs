using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Books;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
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

        public async Task<BookResponse> CreateBookAsync(CreateBookRequest request, CancellationToken ct = default)
        {
            bool isCreated = await _context.Books.AnyAsync(b => b.ISBN == request.ISBN, ct);
            if (isCreated)
                throw new ConflictException("Sach voi ISBN nay da ton tai!");

            bool authorExists = await _context.Authors.AnyAsync(a => a.AuthorId == request.AuthorId, ct);
            bool categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == request.CategoryId, ct);

            if (!authorExists) throw new NotFoundException("Tac gia khong ton tai!");
            if (!categoryExists) throw new NotFoundException("Danh muc khong ton tai!");

            var book = new Book
            {
                PublisherId = request.PublisherId,
                Title = request.Title,
                ISBN = request.ISBN,
                Language = request.Language,
                Description = request.Description,
                CoverImage = request.CoverImage,
                AvailabilityCopies = 0,
                CreatedAt = DateTime.UtcNow,
            };

            book.BookAuthors = new List<BookAuthor> { new() { AuthorId = request.AuthorId } };
            book.BookCategories = new List<BookCategory> { new() { CategoryId = request.CategoryId } };

            await _context.Books.AddAsync(book, ct);
            await _context.SaveChangesAsync(ct);

            return MapToBookResponse(book);
        }

        public async Task UpdateBookAsync(int bookId, UpdateBookRequest request, CancellationToken ct = default)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookCategories)
                .FirstOrDefaultAsync(b => b.BookId == bookId, ct);

            if (book is null)
                throw new NotFoundException("Sach khong ton tai!");

            book.Title = request.Title;
            book.ISBN = request.ISBN;
            book.UpdatedAt = DateTime.UtcNow;

            book.BookAuthors.Clear();
            book.BookAuthors.Add(new BookAuthor { BookId = bookId, AuthorId = request.AuthorId });

            book.BookCategories.Clear();
            book.BookCategories.Add(new BookCategory { BookId = bookId, CategoryId = request.CategoryId });

            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteBookAsync(int bookId, CancellationToken ct = default)
        {
            int deletedRows = await _context.Books
                .Where(b => b.BookId == bookId)
                .ExecuteDeleteAsync(ct);

            if (deletedRows == 0)
                throw new NotFoundException("Sach khong ton tai!");
        }

        public async Task<PaginationResponse<BookResponse>> GetRangeBooksAsync(BookQueryRequest request, CancellationToken ct = default)
        {
            int page = request.Page < 1 ? 1 : request.Page;
            var query = _context.Books.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim();
                query = query.Where(b => b.Title.Contains(kw) || b.ISBN.Contains(kw));
            }

            if (request.CategoryId.HasValue)
                query = query.Where(b => b.BookCategories.Any(bc => bc.CategoryId == request.CategoryId.Value));

            if (request.AuthorId.HasValue)
                query = query.Where(b => b.BookAuthors.Any(ba => ba.AuthorId == request.AuthorId.Value));

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(b => b.BookId)
                .Skip((page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookResponse(
                    b.BookId,
                    b.Title,
                    b.ISBN,
                    b.BookCopies.Count,
                    b.AvailabilityCopies,
                    b.Language,
                    b.Description ?? string.Empty,
                    b.CoverImage ?? string.Empty
                ))
                .ToListAsync(ct);

            return new PaginationResponse<BookResponse>
            {
                Items = items,
                Page = page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }

        public async Task<BookDetailResponse> GetBookByIdAsync(int bookId, CancellationToken ct = default)
        {
            var book = await _context.Books
                .AsNoTracking()
                .Where(b => b.BookId == bookId)
                .Select(b => new BookDetailResponse(
                    b.BookId,
                    b.Title,
                    b.ISBN,
                    b.Language,
                    b.Description ?? string.Empty,
                    b.BookCopies.Count,
                    b.AvailabilityCopies,
                    b.CoverImage ?? string.Empty,
                    b.CreatedAt,
                    b.UpdatedAt
                ))
                .FirstOrDefaultAsync(ct);

            if (book is null)
                throw new NotFoundException("Sach khong ton tai!");

            return book;
        }

        // Private helpers

        private static BookResponse MapToBookResponse(Book book) =>
            new(
                book.BookId,
                book.Title,
                book.ISBN,
                book.BookCopies.Count,
                book.AvailabilityCopies,
                book.Language,
                book.Description ?? string.Empty,
                book.CoverImage ?? string.Empty
            );
    }
}