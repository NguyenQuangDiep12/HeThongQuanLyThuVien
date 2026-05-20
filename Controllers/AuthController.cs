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
            try
            {
                var response = await _authService.RegisterAsync(request, ct);
                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Dang ky thanh cong."
                });
            }
            catch (Exception ex)
            {
                return Conflict(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Data = null,
                    Message = ex.Message
                });
            }
        }

        // POST /api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            try
            {
                var response = await _authService.LoginAsync(request, ct);
                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Dang nhap thanh cong."
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Data = null,
                    Message = ex.Message
                });
            }
        }
    }
}