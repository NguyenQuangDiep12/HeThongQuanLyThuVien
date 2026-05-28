using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    // PUT /users/me/profile
    public record UpdateProfileRequest(
        string FullName, 
        string Phone, 
        string Address, 
        string? AvatarUrl
    );
}
