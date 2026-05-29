using HeThongQuanLyThuVien.DTOs.BookCopies;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC17 - Quản lý bản sao sách (Thêm / Cập nhật trạng thái / Xóa)
    /// </summary>
    public interface IBookCopyService
    {
        // GET /book-copies
        Task<PaginationResponse<BookCopyResponse>> GetBookCopiesAsync(GetBookCopiesRequest request, CancellationToken ct = default);

        // GET /book-copies/:id
        Task<BookCopyDetailResponse> GetBookCopyByIdAsync(int copyId, CancellationToken ct = default);

        // POST /book-copies/book/:id — thêm bản sao cho một đầu sách
        Task<BookCopyResponse> CreateBookCopyAsync(int bookId, CreateBookCopyRequest request, CancellationToken ct = default);

        // PUT /book-copies/:id — cập nhật toàn bộ thông tin bản sao
        Task UpdateBookCopyAsync(int copyId, UpdateBookCopyRequest request, CancellationToken ct = default);

        // PATCH /book-copies/:id/status — chỉ đổi trạng thái (Available/Borrowed/Damaged...)
        Task ChangeBookCopyStatusAsync(int copyId, UpdateBookCopyStatusRequest request, CancellationToken ct = default);

        // DELETE /book-copies/:id  (chỉ Admin)
        Task DeleteBookCopyAsync(int copyId, CancellationToken ct = default);
    }
}