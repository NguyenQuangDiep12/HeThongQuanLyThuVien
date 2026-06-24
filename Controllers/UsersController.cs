using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.DTOs.Users;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET /api/users  (Staff/Admin)
        [HttpGet]
        [Authorize(Roles = "STAFF,ADMIN")] 
        public async Task<IActionResult> GetUsers([FromQuery] GetUserRequest request, CancellationToken ct)
        {
            var currentUserRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _userService.GetUsersAsync(request, currentUserRole, ct);
            return Ok(new ApiResponse<PaginationResponse<UserListInfoResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach nguoi dung thanh cong"
            });
        }

        // GET /api/users/:id  (Owner/Staff/Admin)
        [HttpGet("{id:int}")]
        [Authorize(Roles = "READER,STAFF,ADMIN")]
        public async Task<IActionResult> GetUserDetail([FromRoute] int id, CancellationToken ct)
        {
            // 1. Lay thong tin nguoi dang nhap tu JWT
            string? currentUserIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? role = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(currentUserIdClaim) || string.IsNullOrEmpty(role))
                throw new UnauthorizedException("Nguoi dung chua dang nhap!");

            int currentUserId = int.Parse(currentUserIdClaim);

            var result = await _userService.GetUserByIdAsync(id, currentUserId, role, ct);
            return Ok(new ApiResponse<UserProfileResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay thong tin nguoi dung thanh cong"
            });
        }

        // PUT /api/users/:id  (Staff/Admin cap nhat thong tin nguoi dung)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> UpdateUserInfo([FromRoute] int id, [FromBody] UpdateUserRequest request, CancellationToken ct)
        {
            int currentUserId = int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            string currentRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value!; // lay role trong JWT claims
            await _userService.UpdateUserAsync(id, request, currentUserId ,currentRole, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat thong tin nguoi dung thanh cong"
            });
        }

        // PUT /api/users/me/profile  (Reader tu cap nhat ho so)
        [HttpPut("me/profile")]
        [Authorize(Roles = "READER")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _userService.UpdateMyProfileAsync(currentUserId, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat ho so thanh cong"
            });
        }

        // PATCH /api/users/:id/card-status  (Admin khoa/mo the thu vien)
        [HttpPatch("{id:int}/card-status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateCardStatus([FromRoute] int id, [FromBody] UpdateLibraryCardStatusRequest request, CancellationToken ct)
        {
            await _userService.UpdateLibraryCardStatusAsync(id, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat trang thai the thu vien thanh cong"
            });
        }

        // PATCH /api/users/:id/status  (Admin khoa/mo tai khoan)
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUserStatus([FromRoute] int id, [FromBody] UpdateUserStatusRequest request, CancellationToken ct)
        {
            // Ngan chan viec Admin tu khoa chinh minh (Logic Safety)
            string? currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId != null && int.Parse(currentUserId) == id)
                throw new BadRequestException("Ban khong the tu khoa tai khoan cua chinh minh!");


            await _userService.UpdateUserStatusAsync(id, request,ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat trang thai tai khoan thanh cong"
            });
        }

        // POST /api/users/staff  (Admin tao nhan vien)
        [HttpPost("staff")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request, CancellationToken ct)
        {
            await _userService.CreateStaffAsync(request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Tao tai khoan nhan vien thanh cong"
            });
        }

        // GET /api/users/staffs  (Admin xem danh sach nhan vien)
        [HttpGet("staffs")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetStaffs([FromQuery] GetUserRequest request, CancellationToken ct)
        {
            var result = await _userService.GetStaffsAsync(request, ct);
            return Ok(new ApiResponse<PaginationResponse<UserListInfoResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach nhan vien thanh cong"
            });
        }

        [HttpPut("staff/{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateStaff([FromRoute] int id, [FromBody] UpdateStaffRequest request, CancellationToken ct)
        {
            await _userService.UpdateStaffAsync(id, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat thong tin nhan vien thanh cong"
            });
        }
    }
}