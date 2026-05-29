using HeThongQuanLyThuVien.DTOs.Books;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC06 - Tìm kiếm sách
    /// UC07 - Xem chi tiết sách
    /// UC16 - Quản lý đầu sách (Thêm / Cập nhật / Xóa)
    /// </summary>
    public interface IBookService
    {
        // UC06 - GET /books?keyword=&categoryId=&authorId=&page=&pageSize=
        Task<PaginationResponse<BookResponse>> GetRangeBooksAsync(BookQueryRequest request, CancellationToken ct = default);

        // UC07 - GET /books/:id
        Task<BookDetailResponse> GetBookByIdAsync(int bookId, CancellationToken ct = default);

        // UC16.1 - POST /book
        Task<BookResponse> CreateBookAsync(CreateBookRequest request, CancellationToken ct = default);

        // UC16.2 - PUT /books/:id
        Task UpdateBookAsync(int bookId, UpdateBookRequest request, CancellationToken ct = default);

        // UC16.3 - DELETE /books/:id  (chỉ Admin)
        Task DeleteBookAsync(int bookId, CancellationToken ct = default);
    }
}