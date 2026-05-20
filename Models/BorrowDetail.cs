using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class BorrowDetail
    {
        public int BorrowDetailId { get; set; }
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public ItemBorrowedStatus ItemBorrowedStatus { get; set; } = ItemBorrowedStatus.BORROWING;

        // Navigation
        public BorrowRecord BorrowRecord { get; set; } = null!;
        public Book Book { get; set; } = null!;
        public ICollection<Fine> Fines { get; set; } = new List<Fine>();
    }
}