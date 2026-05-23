using HeThongQuanLyThuVien.DTOs.Books;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    public interface IBookService
    {
        Task<BookResponse> CreateBook(CreateBookRequest request, CancellationToken ct = default);
        Task<bool> UpdateBook(int BookId, UpdateBookRequest request, CancellationToken ct = default);
        Task DeleteBook(int BookId,  CancellationToken ct = default);
        Task<PaginationResponse<BookResponse>> GetRangeBooks(BookQueryRequest request, CancellationToken ct = default);
        Task<BookDetailResponse> GetBookById(int BookId, CancellationToken ct = default);
    }
}
