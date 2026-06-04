using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    public class UpdateBookCopyRequest { 
        public string? ShelfLocation { get; set; } 
        public BookCondition? Condition { get; set; } 
    }
}
