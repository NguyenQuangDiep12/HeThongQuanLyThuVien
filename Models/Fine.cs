using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class Fine
    {
        public int FineId { get; set; }
        public int BorrowingDetailId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CASH;
        public DateTime? PaidAt { get; set; }

        // Navigation
        public BorrowDetail BorrowDetail { get; set; } = null!;
    }
}