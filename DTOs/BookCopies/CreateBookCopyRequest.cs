using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    // CreateBookCopyRequest:
    public class CreateBookCopyRequest {
        [Required]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; }

        public string? ShelfLocation { get; set; }
    }
}
