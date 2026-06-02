using HeThongQuanLyThuVien.DTOs.Books;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC06 - Tim kiem sach
    /// UC07 - Xem chi tiet sach
    /// UC16 - Quan ly dau sach
    ///     + Them sach
    ///     + Cap nhat sach
    ///     + Xoa sach
    /// </summary>
    public interface IBookService
    {
        // UC06 - GET /books
        Task<PaginationResponse<BookResponse>>
            GetRangeBooksAsync(
                BookQueryRequest request,
                CancellationToken ct = default);

        // UC07 - GET /books/:id
        Task<BookDetailResponse>
            GetBookByIdAsync(
                int bookId,
                CancellationToken ct = default);

        // UC16.1 - POST /books
        Task<BookResponse>
            CreateBookAsync(
                CreateBookRequest request,
                CancellationToken ct = default);

        // UC16.2 - PUT /books/:id
        Task<BookResponse>
            UpdateBookAsync(
                int bookId,
                UpdateBookRequest request,
                CancellationToken ct = default);

        // UC16.3 - DELETE /books/:id
        Task DeleteBookAsync(
            int bookId,
            CancellationToken ct = default);
    }
}