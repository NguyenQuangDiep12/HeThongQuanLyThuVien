namespace HeThongQuanLyThuVien.DTOs.Auth
{
    // POST auth/login dang nhap tai khoan
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
