namespace HeThongQuanLyThuVien.DTOs.Auth
{
    // POST auth/login dang nhap tai khoan
    public record LoginRequest(string Email, string Password);
}
