using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.DTOs.Users;
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

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /api/users  (Staff/Admin)
        [HttpGet]
        [Authorize(Roles = "STAFF,ADMIN")] 
        public async Task<IActionResult> GetUsers([FromQuery] GetUserRequest request, CancellationToken ct)
        {
            var result = await _userService.GetUsersAsync(request, ct);
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
            var result = await _userService.GetUserByIdAsync(id, ct);
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
            await _userService.UpdateUserAsync(id, request, ct);
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
            await _userService.UpdateUserStatusAsync(id, request, ct);
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
    }
}