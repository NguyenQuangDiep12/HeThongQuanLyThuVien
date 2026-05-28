namespace HeThongQuanLyThuVien.DTOs.Shared
{
    // Post /auth/login
    public class UserInfoResponse
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
