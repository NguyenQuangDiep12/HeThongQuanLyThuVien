using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.BookCopies;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Services
{
    public class BookCopyService : IBookCopyService
    {
        private readonly ApplicationDbContext _context;
        public BookCopyService(
            ApplicationDbContext context)
        {
            _context = context;
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
        // GET | /book-copies/:id  STAFF/ADMIN
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
        public async Task<CreateBookCopyResponse> CreateBookCopyAsync(int bookId, CreateBookCopyRequest request, CancellationToken ct = default)
        {
            // 1. Kiểm tra đầu sách tồn tại, lấy luôn Title
            var book = await _context.Books
                .AsNoTracking()
                .Where(b => b.BookId == bookId)
                .Select(b => new { b.BookId, b.Title })
                .FirstOrDefaultAsync(ct);

            if (book is null)
                throw new NotFoundException("Đầu sách không tồn tại!");

            // 2. Tạo danh sách bản sao
            var now = DateTime.UtcNow;
            var copies = new List<BookCopy>();

            // Lay so ban sao hien tai
            var currentCopies = await _context.BookCopies.CountAsync(x => x.BookId == bookId ,ct); // 3

            for (int i = 0; i < request.Quantity; i++)
            { // i = 0
                var copyNumber = currentCopies + i + 1; // 3 + 0 + 1 = 4
                copies.Add(new BookCopy
                {
                    BookId = bookId,
                    Barcode = await GenerateUniqueBarcodeAsync(bookId, copyNumber,ct),
                    ShelfLocation = request.ShelfLocation,
                    Status = BookCopyStatus.AVAILABLE,
                    Condition = BookCondition.NORMAL,
                    CreatedAt = now
                });
            }

            await _context.BookCopies.AddRangeAsync(copies, ct);
            await _context.SaveChangesAsync(ct);

            return new CreateBookCopyResponse
            {
                TotalCreated = copies.Count,
                Copies = copies.Select(c => new BookCopyResponse
                {
                    CopyId = c.CopyId,
                    BookId = c.BookId,
                    BookTitle = book.Title,
                    Barcode = c.Barcode,
                    ShelfLocation = c.ShelfLocation ?? string.Empty,
                    Status = c.Status.ToString(),
                    Condition = c.Condition.ToString(),
                    CreatedAt = c.CreatedAt
                }).ToList()
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
            var copy = await _context.BookCopies
                .FirstOrDefaultAsync(bc => bc.CopyId == copyId, ct);

            if (copy is null)
                throw new NotFoundException("Bản sao sách không tồn tại!");

            // Không cho xóa nếu đang được mượn
            if (copy.Status == BookCopyStatus.BORROWED)
                throw new BadRequestException("Không thể xóa bản sao đang được mượn!");

            _context.BookCopies.Remove(copy);

            await _context.SaveChangesAsync(ct);
        }


        // Private Function
        private async Task<string> GenerateUniqueBarcodeAsync(int BookId, int copyNumber,CancellationToken ct = default)
        {
            string barcode;
            int attempt = 0;
            const int maxAttempts = 10;

            do
            {
                if (attempt++ > maxAttempts)
                {
                    throw new ConflictException("Không thể tạo barcode duy nhất sau nhiều lần thử!");
                }

                // Format: BK-BookId:D4-C-copyNumber:D3
                // Y nghia int:D4 dinh dang decimal format bien so nguyen co toi thieu 4 chu so vd 1 => 0001 
                barcode = $"BK-{BookId:D4}-C-{copyNumber:D3}";
            } while (await _context.BookCopies.AnyAsync(c => c.Barcode == barcode, ct));

            return barcode;
        }
    }
}