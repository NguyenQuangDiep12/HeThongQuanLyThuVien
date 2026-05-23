using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    public record UpdateProfileRequest(
        int RoleId,
        string Email,
        string FullName,
        string Password,
        string Phone,
        string Address,
        string LibraryCardCode
    );
}
