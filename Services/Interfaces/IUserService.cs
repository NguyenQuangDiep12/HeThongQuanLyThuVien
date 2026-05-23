using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.DTOs.Users;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginationResponse<UserInfoResponse>> GetAllUser(CancellationToken ct = default);
        Task<UserProfileResponse> GetUserById(int UserId, CancellationToken ct = default);
        Task<UserProfileResponse> UpdateUserById(UpdateProfileRequest request, CancellationToken ct = default);
        Task<UserProfileResponse> UpdateCardStatus(CardStatus Status, CancellationToken ct = default);
        //Task<PaginationResponse<>>
    }
}
