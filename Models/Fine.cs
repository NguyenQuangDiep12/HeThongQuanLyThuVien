using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class Fine
    {
        public int FineId { get; set; }
        public int BorrowDetailId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public FineType FineType { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.PENDING;
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        // Navigation
        public BorrowDetail BorrowDetail { get; set; } = null!;
    }
}