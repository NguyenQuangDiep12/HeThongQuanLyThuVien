using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Auth
{
    public class ResetPasswordRequest { 
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")] 
        public string Password { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare(nameof(Password), ErrorMessage = "Xác nhận mật khẩu không khớp")] 
        public string ConfirmPassword { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống")] 
        public string OldPassword { get; set; } = string.Empty; 
    }
}
