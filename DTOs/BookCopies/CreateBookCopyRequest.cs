using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    // CreateBookCopyRequest:
    public class CreateBookCopyRequest { 
        [Required(ErrorMessage = "Barcode không được để trống")] 
        public string Barcode { get; set; } = string.Empty; 
        public string? ShelfLocation { get; set; } 
    }
}
