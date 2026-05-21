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
            var IsCreated = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == request.ISBN);
            if(IsCreated != null)
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

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

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
            var existsBook = await _context.Books.FirstOrDefaultAsync(b => b.BookId == BookId);
            if(existsBook == null)
            {
                throw new Exception("Book not found!");
            }

            _context.Books.Remove(existsBook);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginationResponse<BookResponse>> GetRangeBooks(PaginationRequest request, CancellationToken ct = default)
        {
            var query = _context.Books.AsNoTracking().OrderBy(b => b.BookId);

            // tong so sach
            var total = await query.CountAsync(ct);

            var getRangeBook = await query
                .Skip((request.Page - 1) * request.PageSize)
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
                )).ToListAsync();

            return new PaginationResponse<BookResponse>
            {
                Items = getRangeBook,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }
        public async Task<bool> UpdateBook(int BookId, UpdateBookRequest request, CancellationToken ct = default)
        {
            // Kiem tra xem co ton tai sach hay khong 
            var existsBook = await _context.Books.FirstOrDefaultAsync(b => b.BookId==BookId);
            if(existsBook == null)
            {
                throw new Exception("Book Not Found!");
            }

            // Update truc tiep tren entity dang query 
            // EF core dang tracking truc tiep tren entity do
            existsBook.Title = request.Title;
            existsBook.ISBN = request.ISBN;
            existsBook.CategoryId = request.CategoryId;
            existsBook.AuthorId = request.AuthorId;
            existsBook.PublisherId = request.PublisherId;

            await _context.SaveChangesAsync(ct);

            return true;
        }
        
        public async Task<BookDetailResponse> GetBookById(int BookId, CancellationToken ct = default)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == BookId, ct);

            if(book == null)
            {
                throw new Exception("Book Not Found!");
            }

            return new BookDetailResponse(
                    book.BookId,
                    book.Title,
                    book.ISBN,
                    book.Language,
                    book.Description,
                    book.Quantity,
                    book.AvailableQuantity,
                    book.CoverImage,
                    book.CreatedAt,
                    book.UpdatedAt
                );
        }
    }
}
