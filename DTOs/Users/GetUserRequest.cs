using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    public class GetUserRequest : PaginationRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public UserStatus? Status { get; set; }
        public string? RoleName { get; set; }
    }
}
