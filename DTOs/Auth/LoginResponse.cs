using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.DTOs.Auth
{
    // Response for POST auth/login
    public record LoginResponse(string AccessToken, UserInfoResponse UserInfo);
}
