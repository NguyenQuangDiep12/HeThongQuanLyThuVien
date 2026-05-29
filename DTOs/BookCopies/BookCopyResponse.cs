using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    public class BookCopyResponse
    {
        public int CopyId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string ShelfLocation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public bool IsReferenceOnly { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
