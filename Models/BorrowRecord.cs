using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class BorrowRecord
    {
        public int BorrowId { get; set; }
        public int ReaderId { get; set; }
        public int? ApprovedBy { get; set; }
        public string BorrowCode { get; set; } = string.Empty;
        public DateTime? BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public ExtensionRequestStatus ExtensionRequestStatus { get; set; }
        public int ExtensionCount { get; set; }
        public BorrowType BorrowType { get; set; }
        public BorrowStatus Status { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        // Navigation
        public User Reader { get; set; } = null!;
        public User? Approver { get; set; }
        public ICollection<BorrowDetail> BorrowDetails { get; set; } = new List<BorrowDetail>();
    }
}