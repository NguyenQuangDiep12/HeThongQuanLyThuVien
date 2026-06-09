using HeThongQuanLyThuVien.DTOs.BorrowRecords;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/borrow-records")]
    public class BorrowRecordsController : ControllerBase
    {
        private readonly IBorrowRecordService _borrowRecordService;
        private readonly IHttpContextAccessor _contextAccessor;

        public BorrowRecordsController(
            IBorrowRecordService borrowRecordService,
            IHttpContextAccessor contextAccessor)
        {
            _borrowRecordService = borrowRecordService;
            _contextAccessor = contextAccessor;
        }

        // GET /api/borrow-records  (Staff/Admin) | GET | /borrow-records | Danh sách phiếu mượn | Staff/Admin |
        [HttpGet]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> GetListBorrowRecords([FromQuery] BorrowRecordQueryRequest request, CancellationToken ct)
        {
            var result = await _borrowRecordService.GetBorrowRecordsAsync(request, ct);
            return Ok(new ApiResponse<PaginationResponse<BorrowRecordSummaryResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach phieu muon thanh cong"
            });
        }

        // GET /api/users/:id/borrow-records  (Owner/Staff/Admin) | GET | /users/:id/borrow-records | Lịch sử mượn sách của người dùng | Owner/Staff/Admin |
        // Route dat o day vi dung prefix /api/users/:id
        [HttpGet("/api/users/{id:int}/borrow-records")]
        [Authorize(Roles = "READER,STAFF,ADMIN")]
        public async Task<IActionResult> GetUserBorrowRecords([FromRoute] int id, [FromQuery] PaginationRequest request, CancellationToken ct)
        {
            int currentUserId = int.Parse(_contextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string currentRole = _contextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value;

            var result = await _borrowRecordService.GetUserBorrowRecordsAsync(id, currentUserId, currentRole,request, ct);
            return Ok(new ApiResponse<PaginationResponse<BorrowRecordSummaryResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay lich su muon sach thanh cong"
            });
        }

        // GET /api/borrow-records/:id  (Owner/Staff/Admin)  | GET | /borrow-records/:id | Chi tiết phiếu mượn | Owner/Staff/Admin |
        [HttpGet("{id:int}")]
        [Authorize(Roles = "READER,STAFF,ADMIN")]
        public async Task<IActionResult> GetDetailBorrowRecord([FromRoute] int id, CancellationToken ct)
        {

            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string currentRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var result = await _borrowRecordService.GetBorrowRecordByIdAsync(id, currentUserId, currentRole, ct);
            return Ok(new ApiResponse<BorrowRecordDetailResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet phieu muon thanh cong"
            });
        }

        // POST /api/borrow-records  (Staff/Admin tao phieu muon) | POST | /borrow-records | Tạo phiếu mượn mới | Staff/Admin |
        [HttpPost]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> CreateBorrowRecord([FromBody] CreateBorrowRecordRequest request, CancellationToken ct)
        {
            int staffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _borrowRecordService.CreateBorrowRecordAsync(staffId, request, ct);
            return Ok(new ApiResponse<BorrowRecordDetailResponse>
            {
                Success = true,
                Data = result,
                Message = "Tao phieu muon thanh cong"
            });
        }

        // POST /api/borrow-records/:id/extension-requests  (Reader gui yeu cau gia han) | POST | /borrow-records/:id/extension-requests | Gửi yêu cầu gia hạn sách | Owner |
        [HttpPost("{id:int}/extension-requests")]
        [Authorize(Roles = "READER")]
        public async Task<IActionResult> SubmitBookRenewal([FromRoute] int id, CancellationToken ct)
        {
            int readerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _borrowRecordService.SubmitExtensionRequestAsync(id, readerId, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Gui yeu cau gia han thanh cong"
            });
        }

        // PATCH /api/borrow-records/:id/return  (Staff/Admin xac nhan tra sach) | PATCH | /borrow-records/:id/return | Xác nhận trả sách | Staff/Admin |
        [HttpPatch("{id:int}/return")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> ConfirmBookReturned([FromRoute] int id, [FromBody] ConfirmReturnRequest request, CancellationToken ct)
        {
            await _borrowRecordService.ConfirmReturnAsync(id, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Xac nhan tra sach thanh cong"
            });
        }

        // PATCH /api/borrow-records/:id/cancel  (Staff/Admin huy phieu muon) | PATCH | /borrow-records/:id/cancel | Hủy phiếu mượn | /Staff/Admin |
        [HttpPatch("{id:int}/cancel")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> CancelBorrowRecord([FromRoute] int id, CancellationToken ct)
        {
            await _borrowRecordService.CancelBorrowRecordAsync(id, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Huy phieu muon thanh cong"
            });
        }

        // PATCH /api/borrow-records/:id/extend  (Staff/Admin xac nhan gia han) | PATCH | /borrow-records/:id/extend | Gia hạn sách | Staff/Admin |
        [HttpPatch("{id:int}/extend")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> ConfirmBookRenewal([FromRoute] int id, [FromBody] ProcessExtensionRequest request ,CancellationToken ct)
        {

            await _borrowRecordService.ConfirmExtensionAsync(id, request ,ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Gia han sach thanh cong"
            });
        }
    }
}