namespace HeThongQuanLyThuVien.DTOs.Auth
{
    // GET /auth/register
    public record RegisterRequest(string FullName, string Email, string Password, string Phone);
}
