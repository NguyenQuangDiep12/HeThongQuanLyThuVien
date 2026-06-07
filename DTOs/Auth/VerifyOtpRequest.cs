using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Auth
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP không được để trống")]
        public string Otp { get; set; } = string.Empty;
    }
}
