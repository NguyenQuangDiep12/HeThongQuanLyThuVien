using HeThongQuanLyThuVien.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Fines
{
    public class CreateFineRequest { 
        [Range(1, int.MaxValue)] 
        public int BorrowDetailId { get; set; } 
        [Required] 
        public FineType FineType { get; set; }
        [Range(1000, 1000000, ErrorMessage = "Số tiền phạt phải từ 1,000 đến 1,000,000 VNĐ")]
        public decimal Amount { get; set; } 
        public string? Reason { get; set; } 
    }
}
