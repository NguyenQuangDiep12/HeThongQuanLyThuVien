using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class BorrowDetail
    {
        public int BorrowDetailId { get; set; }
        public int BorrowId { get; set; }
        public int CopyId { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public BookCondition? ItemCondition { get; set; }
        public BorrowDetailStatus Status { get; set; } = BorrowDetailStatus.Borrowing;
        // Navigation
        public BorrowRecord BorrowRecord { get; set; } = null!;
        public BookCopy BookCopy { get; set; } = null!;
        public ICollection<Fine> Fines { get; set; } = new List<Fine>();
    }
}