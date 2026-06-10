using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.DTOs.Users;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC05  - Xem và cập nhật hồ sơ cá nhân (Reader tự cập nhật)
    /// UC21  - Quản lý người dùng (Staff / Admin: xem, cập nhật, khóa/mở tài khoản)
    /// UC22  - Quản lý nhân viên (Admin: thêm, cập nhật Staff)
    /// </summary>
    public interface IUserService
    {
        // UC21 — GET /users  (Staff/Admin)
        Task<PaginationResponse<UserListInfoResponse>> GetUsersAsync(GetUserRequest request, string currentUserRole, CancellationToken ct = default);

        // UC21 — GET /users/:id  (Owner/Staff/Admin)
        Task<UserProfileResponse> GetUserByIdAsync(int userId, int currentUserId, string currentRole ,CancellationToken ct = default);

        // UC21 — PUT /users/:id  (Staff/Admin cập nhật thông tin người dùng)
        Task UpdateUserAsync(int userId, UpdateUserRequest request, int currentUserId, string currentRole, CancellationToken ct = default);

        // UC05  — PUT /users/me/profile  (Reader tự cập nhật hồ sơ)
        Task UpdateMyProfileAsync(int currentUserId, UpdateProfileRequest request, CancellationToken ct = default);

        // UC21 — PATCH /users/:id/status  (Admin khóa / mở tài khoản)
        Task UpdateUserStatusAsync(int userId, UpdateUserStatusRequest request, CancellationToken ct = default);

        // UC21 — PATCH /users/:id/card-status  (Admin khóa / mở thẻ thư viện)
        Task UpdateLibraryCardStatusAsync(int userId, UpdateLibraryCardStatusRequest request, CancellationToken ct = default);

        // UC22 — POST /staff  (Admin tạo tài khoản nhân viên)
        Task CreateStaffAsync(CreateStaffRequest request, CancellationToken ct = default);

        // UC22 — GET /staffs  (Admin xem danh sách nhân viên)
        Task<PaginationResponse<UserListInfoResponse>> GetStaffsAsync(GetUserRequest request, CancellationToken ct = default);

        // UC22 — PUT /staff/:id  (Admin cập nhật thông tin nhân viên)
        Task UpdateStaffAsync(int staffId, UpdateStaffRequest request, CancellationToken ct = default);
    }
}