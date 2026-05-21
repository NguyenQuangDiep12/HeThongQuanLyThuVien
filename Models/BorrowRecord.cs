using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class BorrowRecord
    {
        public int BorrowId { get; set; }
        public int ReaderId { get; set; }
        public int? ApprovedBy { get; set; }
        public string BorrowCode { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public BorrowStatus Status { get; set; } = BorrowStatus.PENDING;
        public DateTime CreatedAt { get; set; }
        // Navigation
        public User Reader { get; set; } = null!;
        public User Approver { get; set; } = null!;
        public ICollection<BorrowDetail> BorrowDetails { get; set; } = new List<BorrowDetail>();
    }
}