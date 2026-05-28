using HeThongQuanLyThuVien.DTOs.Auth;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var response = await _authService.RegisterAsync(request, ct);
            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Data = response,
                Message = "Dang ky thanh cong"
            });
        }

        // POST /api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var response = await _authService.LoginAsync(request, ct);
            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Data = response,
                Message = "Danh nhap thanh cong"
            });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
        {
            await _authService.ForgotPasswordAsync(request, ct);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Da gui mat khau moi qua email"
            });
        }

        [Authorize(Roles = "READER,STAFF,ADMIN")]
        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
        {
            await _authService.ResetPasswordAsync(request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Doi mat khau thanh cong"
            });
        }
    }
}