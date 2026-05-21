using HeThongQuanLyThuVien.DTOs.Authors;
using HeThongQuanLyThuVien.DTOs.Categories;
using HeThongQuanLyThuVien.DTOs.Publishers;

namespace HeThongQuanLyThuVien.DTOs.Books
{
    public record BookDetailResponse(
        int BookId, 
        string Title, 
        string ISBN, 
        string Language, 
        string Description, 
        int Quantity,
        int AvailableQuantity, 
        string CoverImage, 
        DateTime CreatedAt, 
        DateTime? UpdatedAt
        );
        // GET /books/:id
        // chen full object vi api lay chi tiet toan bo thong tin cua sach
}
