using HeThongQuanLyThuVien.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    public class UpdateBookCopyStatusRequest { 
        [Required(ErrorMessage = "Trạng thái không được để trống")] 
        public BookCopyStatus Status { get; set; } 
    }
}
