using HeThongQuanLyThuVien.DTOs.Auth;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default);
        Task VerifiyOtpAsync(VerifyOtpRequest request, CancellationToken ct = default);
        Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
    }
}