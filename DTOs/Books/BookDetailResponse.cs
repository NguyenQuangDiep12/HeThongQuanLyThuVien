using HeThongQuanLyThuVien.DTOs.Authors;
using HeThongQuanLyThuVien.DTOs.Categories;
using HeThongQuanLyThuVien.DTOs.Publishers;

namespace HeThongQuanLyThuVien.DTOs.Books
{
    // GET /books/:id
    public record BookDetailResponse(
        int BookId,

        string Title,

        string ISBN,

        string Language,

        string Description,

        // Tong so ban sao
        int TotalCopies,

        // So ban sao dang available
        int AvailableCopies,

        string CoverImage,

        DateTime CreatedAt,

        DateTime? UpdatedAt,

        // Full thong tin lien quan
        PublisherResponse Publisher,

        List<AuthorResponse> Authors,

        List<CategoryResponse> Categories
    );
    // GET /books/:id
    // chen full object vi api lay chi tiet toan bo thong tin cua sach
}
