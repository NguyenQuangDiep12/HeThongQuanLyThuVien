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

        public BorrowRecordsController(IBorrowRecordService borrowRecordService)
        {
            _borrowRecordService = borrowRecordService;
        }

        // GET /api/borrow-records  (Staff/Admin)
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

        // GET /api/users/:id/borrow-records  (Owner/Staff/Admin)
        // Route dat o day vi dung prefix /api/users/:id
        [HttpGet("/api/users/{id:int}/borrow-records")]
        [Authorize(Roles = "READER,STAFF,ADMIN")]
        public async Task<IActionResult> GetUserBorrowRecords([FromRoute] int id, [FromQuery] PaginationRequest request, CancellationToken ct)
        {
            var result = await _borrowRecordService.GetUserBorrowRecordsAsync(id, request, ct);
            return Ok(new ApiResponse<PaginationResponse<BorrowRecordSummaryResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay lich su muon sach thanh cong"
            });
        }

        // GET /api/borrow-records/:id  (Owner/Staff/Admin)
        [HttpGet("{id:int}")]
        [Authorize(Roles = "READER,STAFF,ADMIN")]
        public async Task<IActionResult> GetDetailBorrowRecord([FromRoute] int id, CancellationToken ct)
        {
            var result = await _borrowRecordService.GetBorrowRecordByIdAsync(id, ct);
            return Ok(new ApiResponse<BorrowRecordDetailResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet phieu muon thanh cong"
            });
        }

        // POST /api/borrow-records  (Staff/Admin tao phieu muon)
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

        // POST /api/borrow-records/:id/extension-requests  (Reader gui yeu cau gia han)
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

        // PATCH /api/borrow-records/:id/return  (Staff/Admin xac nhan tra sach)
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

        // PATCH /api/borrow-records/:id/cancel  (Owner/Staff/Admin huy phieu muon)
        [HttpPatch("{id:int}/cancel")]
        [Authorize(Roles = "READER,STAFF,ADMIN")]
        public async Task<IActionResult> CancelLoanPass([FromRoute] int id, CancellationToken ct)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string currentRole = User.FindFirst(ClaimTypes.Role)!.Value;
            await _borrowRecordService.CancelBorrowRecordAsync(id, currentUserId, currentRole, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Huy phieu muon thanh cong"
            });
        }

        // PATCH /api/borrow-records/:id/extend  (Staff/Admin xac nhan gia han)
        [HttpPatch("{id:int}/extend")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> ConfirmBookRenewal([FromRoute] int id, CancellationToken ct)
        {
            int staffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _borrowRecordService.ConfirmExtensionAsync(id, staffId, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Gia han sach thanh cong"
            });
        }
    }
}