using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    // PATCH /users/:id/card-status
    public record UpdateUserStatusRequest(
            UserStatus Status
        );
}
