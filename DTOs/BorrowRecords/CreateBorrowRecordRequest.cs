using HeThongQuanLyThuVien.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    public class CreateBorrowRecordRequest { 
        [Range(1, int.MaxValue, ErrorMessage = "ReaderId không hợp lệ")] 
        public int ReaderId { get; set; } 
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 bản sao")] 
        public List<int> CopyIds { get; set; } = new List<int>(); 
        [Required(ErrorMessage = "Loại mượn không được để trống")] 
        public BorrowType BorrowType { get; set; } 
    }
}
