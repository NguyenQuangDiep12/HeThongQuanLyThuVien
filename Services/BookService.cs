using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Authors;
using HeThongQuanLyThuVien.DTOs.Books;
using HeThongQuanLyThuVien.DTOs.Categories;
using HeThongQuanLyThuVien.DTOs.Publishers;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HeThongQuanLyThuVien.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST /books
        public async Task<BookResponse> CreateBookAsync(CreateBookRequest request,CancellationToken ct = default)
        {
            // Kiem tra ISBN
            bool isExists = await _context.Books.AnyAsync(b => b.ISBN == request.ISBN, ct);
            if (isExists)
            {
                throw new ConflictException("Sach voi ISBN nay da ton tai!");
            }
            // Validate Author
            var validAuthorCount = await _context.Authors.CountAsync(a => request.AuthorIds.Contains(a.AuthorId), ct);

            if (validAuthorCount != request.AuthorIds.Count)
            {
                throw new NotFoundException("Một hoặc nhiều tác giả không tồn tại!");
            }
            // Validate Category
            var validCategoryCount = await _context.Categories.CountAsync(c => request.CategoryIds.Contains(c.CategoryId), ct);

            if (validCategoryCount != request.CategoryIds.Count)
            {
                throw new NotFoundException("Một hoặc nhiều danh mục không tồn tại!");
            }
            // Validate Publisher
            bool publisherExists = await _context.Publisher.AnyAsync(p => p.PublisherId == request.PublisherId, ct);

            if (!publisherExists)
            {
                throw new NotFoundException("Nhà xuất bản không tồn tại!");
            }

            // Tao dau sach
            var book = new Book
            {
                Title = request.Title,
                ISBN = request.ISBN,
                PublisherId = request.PublisherId,
                Language = request.Language,
                Description = request.Description,
                CoverImage = request.CoverImage,
                CreatedAt = DateTime.UtcNow
            };

            // Gan tac gia
            book.BookAuthors = request.AuthorIds
                .Distinct()
                .Select(AuthorId => new BookAuthor
                {
                    AuthorId = AuthorId
                }).ToList();

            // Gan danh muc
            book.BookCategories = request.CategoryIds
                .Distinct()
                .Select(CategoryId => new BookCategory
                {
                    CategoryId = CategoryId
                }).ToList();

            await _context.Books.AddAsync(book, ct);

            await _context.SaveChangesAsync(ct);

            return new BookResponse(
                book.BookId,
                book.Title,
                book.ISBN,
                // TotalCopies
                0,

                // AvailableCopies
                0,
                book.Language ?? string.Empty,
                book.Description ?? string.Empty,
                book.CoverImage ?? string.Empty
            );
        }

        // PATCH /books/:id
        public async Task<BookResponse> UpdateBookAsync(int bookId, UpdateBookRequest request, CancellationToken ct = default)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookCategories)
                .Include(b => b.BookCopies)
                .FirstOrDefaultAsync(
                    b => b.BookId == bookId,
                    ct);

            if (book is null)
            {
                throw new NotFoundException(
                    "Sach khong ton tai!");
            }
            // Kiem tra ISBN trung
            bool duplicatedISBN = await _context.Books
                .AnyAsync(b =>
                    b.BookId != bookId &&
                    b.ISBN == request.ISBN,
                    ct);
            if (duplicatedISBN)
            {
                throw new ConflictException(
                    "ISBN da duoc su dung!");
            }
            // Validate Author
            var authorIds = request.AuthorIds.Distinct().ToList();

            var authorCount = await _context.Authors.CountAsync(a => request.AuthorIds.Contains(a.AuthorId), ct);

            if (authorCount != request.AuthorIds.Count)
            {
                throw new NotFoundException("Mot hoac nhieu tac gia khong ton tai!");
            }
            // Validate Category
            var cateogoryIds = request.CategoryIds.Distinct().ToList();

            var categoryCount = await _context.Categories.CountAsync(c => request.CategoryIds.Contains(c.CategoryId), ct);

            if (categoryCount != request.CategoryIds.Count)
            {
                throw new NotFoundException("Mot hoac nhieu danh muc khong ton tai!");
            }
            // Validate Publisher
            bool publisherExists = await _context.Publisher
                .AnyAsync(p => p.PublisherId == request.PublisherId, ct);
            if (!publisherExists)
            {
                throw new NotFoundException("Nha xuat ban khong ton tai!");
            }
            // Update thong tin
            book.Title = request.Title;
            book.ISBN = request.ISBN;
            book.PublisherId = request.PublisherId;
            book.Language = request.Language;
            book.Description = request.Description;
            book.CoverImage = request.CoverImage;
            book.UpdatedAt = DateTime.UtcNow;

            // Update tac gia
            _context.BookAuthors.RemoveRange(book.BookAuthors);

            book.BookAuthors = authorIds.Select(authorId => new BookAuthor
            {
                BookId = bookId,
                AuthorId = authorId
            }).ToList();

            // Update danh muc
            _context.BookCategories.RemoveRange(book.BookCategories);

            book.BookCategories = cateogoryIds.Select(categoryId => new BookCategory
            {
                BookId = bookId,
                CategoryId = categoryId
            }).ToList();


            await _context.SaveChangesAsync(ct);
            return new BookResponse(
                book.BookId,
                book.Title,
                book.ISBN,
                // TotalCopies
                book.BookCopies.Count,
                // AvailableCopies
                book.BookCopies.Count(bc => bc.Status == BookCopyStatus.AVAILABLE),
                book.Language ?? string.Empty,
                book.Description ?? string.Empty,
                book.CoverImage ?? string.Empty
            );
        }

        // DELETE /books/:id
        public async Task DeleteBookAsync(int bookId, CancellationToken ct = default)
        {
            var book = await _context.Books
                .Include(b => b.BookCopies)
                .FirstOrDefaultAsync(
                    b => b.BookId == bookId,
                    ct);
            if (book is null)
            {
                throw new NotFoundException(
                    "Sach khong ton tai!");
            }
            // Khong cho xoa neu con sach dang muon
            bool hasBorrowedCopies = book.BookCopies.Any(bc =>
                bc.Status == BookCopyStatus.BORROWED);
            if (hasBorrowedCopies)
            {
                throw new BadRequestException(
                    "Khong the xoa sach dang duoc muon!");
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync(ct);
        }

        // GET /books
        public async Task<PaginationResponse<BookResponse>> GetRangeBooksAsync(BookQueryRequest request, CancellationToken ct = default)
        {
            IQueryable<Book> query = _context.Books.AsNoTracking();
            // Search keyword
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                string keyword = request.Keyword.Trim();
                query = query.Where(b =>
                    b.Title.Contains(keyword) ||
                    b.ISBN.Contains(keyword) ||
                    b.BookAuthors.Any(ba =>
                        ba.Author.AuthorName.Contains(keyword)));
            }
            // Filter category
            if (request.CategoryId.HasValue)
            {
                query = query.Where(b =>
                    b.BookCategories.Any(bc =>
                        bc.CategoryId == request.CategoryId.Value));
            }
            // Filter author
            if (request.AuthorId.HasValue)
            {
                query = query.Where(b =>
                    b.BookAuthors.Any(ba =>
                        ba.AuthorId == request.AuthorId.Value));
            }
            int totalRecords = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookResponse(
                    b.BookId,
                    b.Title,
                    b.ISBN,

                    // TotalCopies
                    b.BookCopies.Count(),

                    // AvailableCopies
                    b.BookCopies.Count(bc =>
                        bc.Status == BookCopyStatus.AVAILABLE),

                    b.Language ?? string.Empty,
                    b.Description ?? string.Empty,
                    b.CoverImage ?? string.Empty
                ))
                .ToListAsync(ct);
            return new PaginationResponse<BookResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
        }
        // GET /books/:id
        public async Task<BookDetailResponse> GetBookByIdAsync(int bookId, CancellationToken ct = default)
        {
            var book = await _context.Books
                .AsNoTracking()
                .Where(b => b.BookId == bookId)
                .Select(b => new BookDetailResponse(
                    b.BookId,
                    b.Title,
                    b.ISBN,
                    b.Language ?? string.Empty,
                    b.Description ?? string.Empty,
                    // Tong so ban sao
                    b.BookCopies.Count(),
                    // So ban sao available
                    b.BookCopies.Count(bc =>
                        bc.Status == BookCopyStatus.AVAILABLE),
                    b.CoverImage ?? string.Empty,
                    b.CreatedAt,
                    b.UpdatedAt,
                    // Publisher
                    new PublisherResponse(
                        b.Publisher.PublisherId,
                        b.Publisher.PublisherName,
                        b.Publisher.LogoUrl
                    ),
                    // Authors
                    b.BookAuthors
                        .Select(ba => new AuthorResponse(
                            ba.Author.AuthorId,
                            ba.Author.AuthorName,
                            ba.Author.Biography,
                            ba.Author.AuthorUrl
                        )).ToList(),
                    // Categories
                    b.BookCategories
                        .Select(bc => new CategoryResponse(
                            bc.Category.CategoryId,
                            bc.Category.CategoryName,
                            bc.Category.Description
                        )).ToList()
                )).FirstOrDefaultAsync(ct);
            if (book is null)
            {
                throw new NotFoundException(
                    "Sach khong ton tai!");
            }
            return book;
        }
    }
}