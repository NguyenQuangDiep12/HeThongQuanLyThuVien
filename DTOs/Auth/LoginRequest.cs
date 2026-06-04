using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Auth
{
    // POST auth/login dang nhap tai khoan
    public class LoginRequest { 
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")] 
        public string Email { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")] 
        public string Password { get; set; } = string.Empty; 
    }
}
