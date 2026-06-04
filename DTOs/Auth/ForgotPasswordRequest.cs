using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Auth
{
    public class ForgotPasswordRequest { 
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")] 
        public string Email { get; set; } = string.Empty; 
    }
}
