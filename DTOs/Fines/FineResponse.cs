using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.Fines
{
    public class FineResponse
    {
        public int FineId { get; set; }

        public int BorrowDetailId { get; set; }

        public string BorrowCode { get; set; } = null!;

        public int ReaderId { get; set; }

        public string ReaderName { get; set; } = null!;

        public decimal Amount { get; set; }

        public string? Reason { get; set; }

        public FineType FineType { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}