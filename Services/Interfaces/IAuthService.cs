using HeThongQuanLyThuVien.DTOs.Auth;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}