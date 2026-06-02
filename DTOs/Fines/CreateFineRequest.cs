using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.Fines
{
    public class CreateFineRequest
    {
        public int BorrowDetailId { get; set; }
        public FineType FineType { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
    }
}
