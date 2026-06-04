using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Auth
{
    // GET /auth/register
    public class RegisterRequest { 
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 100 ký tự")] 
        public string FullName { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")] 
        public string Email { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")] 
        public string Password { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")] 
        public string Phone { get; set; } = string.Empty; 
    }
}
