using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.DTOs.Users;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    public interface IUserService
    {
        // GET /users
        Task<PaginationResponse<UserListInfoResponse>> GetUsersAsync(int page, int pageSize, string? keyword );
        // GET /users/:id
        Task<UserProfileResponse?> GetUserByIdAsync(int userId);
        // PUT /users/:id
        Task UpdateUserAsync(int userId, UpdateUserRequest request); 
        // PUT /users/me/profile
        Task UpdateMyProfileAsync(int currentUserId, UpdateProfileRequest request);
        // PATCH /users/:id/status
        Task UpdateUserStatusAsync(int userId, UpdateUserStatusRequest request ); 
        // PATCH /users/:id/card-status
        Task UpdateLibraryCardStatusAsync(int userId, UpdateLibraryCardStatusRequest request );
        // POST /staff
        Task CreateStaffAsync(CreateStaffRequest request);
        // GET /staffs
        Task<PaginationResponse<UserListInfoResponse>> GetStaffsAsync( int page, int pageSize, string? keyword );
        // PUT /staff/:id
        Task UpdateStaffAsync(int staffId, UpdateStaffRequest request);
    }
}
