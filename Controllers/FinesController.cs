using HeThongQuanLyThuVien.DTOs.Fines;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/fines")]
    public class FinesController : ControllerBase
    {
        private readonly IFineService _fineService;

        public FinesController(IFineService fineService)
        {
            _fineService = fineService;
        }

        // GET /api/fines  (Staff/Admin)
        [HttpGet]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> GetListFines([FromQuery] PaginationRequest request, CancellationToken ct)
        {
            var result = await _fineService.GetFinesAsync(request, ct);
            return Ok(new ApiResponse<PaginationResponse<FineResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach phieu phat thanh cong"
            });
        }

        // GET /api/fines/:id  (Staff/Admin)
        [HttpGet("{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> GetDetailFine([FromRoute] int id, CancellationToken ct)
        {
            var result = await _fineService.GetFineByIdAsync(id, ct);
            return Ok(new ApiResponse<FineResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet phieu phat thanh cong"
            });
        }

        // GET /api/users/:id/fines  (Staff/Admin theo doi vi pham cua nguoi dung)
        [HttpGet("/api/users/{id:int}/fines")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> MonitorUserViolations([FromRoute] int id, [FromQuery] PaginationRequest request, CancellationToken ct)
        {
            var result = await _fineService.GetUserFinesAsync(id, request, ct);
            return Ok(new ApiResponse<PaginationResponse<FineResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach vi pham nguoi dung thanh cong"
            });
        }

        // POST /api/fines  (Staff/Admin tao phieu phat)
        [HttpPost]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> CreateFineTicket([FromBody] CreateFineRequest request, CancellationToken ct)
        {
            var result = await _fineService.CreateFineAsync(request, ct);
            return Ok(new ApiResponse<FineResponse>
            {
                Success = true,
                Data = result,
                Message = "Tao phieu phat thanh cong"
            });
        }

        // PATCH /api/fines/:id/pay  (Staff/Admin xac nhan thanh toan)
        [HttpPatch("{id:int}/pay")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> PaymentConfirmation([FromRoute] int id, CancellationToken ct)
        {
            await _fineService.PayFineAsync(id, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Xac nhan thanh toan phieu phat thanh cong"
            });
        }
    }
}