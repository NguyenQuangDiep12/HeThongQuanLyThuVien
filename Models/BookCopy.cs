using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class BookCopy
    {
        public int CopyId { get; set; }
        public int BookId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string? ShelfLocation { get; set; }
        public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;
        public BookCondition Condition { get; set; } = BookCondition.Normal;
        public bool IsReferenceOnly { get; set; }
        public DateTime CreatedAt { get; set; }
        // Navigation
        public Book Book { get; set; } = null!;
        public ICollection<BorrowDetail> BorrowDetails { get; set; } = new List<BorrowDetail>();
    }
}
