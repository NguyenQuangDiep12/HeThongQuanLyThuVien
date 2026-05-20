using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.DTOs.Auth
{
    // Response for POST auth/login
    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;
        public UserInfoResponse UserInfo { get; set; } = null!;
    }
}
