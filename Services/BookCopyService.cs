using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.BookCopies;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

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
                    IsReferenceOnly = bc.IsReferenceOnly,
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
                    IsReferenceOnly = bc.IsReferenceOnly,
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

        public Task<BookCopyResponse> CreateBookCopyAsync(int bookId, CreateBookCopyRequest request, CancellationToken ct = default)
        {
            
        }

        public Task UpdateBookCopyAsync(int copyId, UpdateBookCopyRequest request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task ChangeBookCopyStatusAsync(int copyId, UpdateBookCopyStatusRequest request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBookCopyAsync(int copyId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}