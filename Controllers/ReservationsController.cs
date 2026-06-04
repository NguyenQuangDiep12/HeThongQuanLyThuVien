using HeThongQuanLyThuVien.DTOs.Reservations;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // GET /api/reservations  (Staff/Admin)
        [HttpGet]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> GetListReservations([FromQuery] PaginationRequest request, CancellationToken ct)
        {
            var result = await _reservationService.GetReservationsAsync(request, ct);
            return Ok(new ApiResponse<PaginationResponse<ReservationResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach dat truoc thanh cong"
            });
        }

        // POST /api/reservations  (Staff/Admin tao phieu dat truoc)
        [HttpPost]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> CreatePreOrderBookForm([FromBody] CreateReservationRequest request, CancellationToken ct)
        {
            var result = await _reservationService.CreateReservationAsync(request, ct);
            return Ok(new ApiResponse<ReservationResponse>
            {
                Success = true,
                Data = result,
                Message = "Tao phieu dat truoc thanh cong"
            });
        }

        // PATCH /api/reservations/:id/cancel  (Staff/Admin huy dat truoc)
        [HttpPatch("{id:int}/cancel")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> CancelPreOrder([FromRoute] int id, CancellationToken ct)
        {
            await _reservationService.CancelReservationAsync(id, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Huy dat truoc thanh cong"
            });
        }

        // PATCH /api/reservations/:id/complete  (Staff/Admin chuyen dat truoc thanh phieu muon)
        [HttpPatch("{id:int}/complete")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> CompleteReservation([FromRoute] int id, CancellationToken ct)
        {
            int staffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            int borrowId = await _reservationService.CompleteReservationAsync(id, staffId, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { BorrowId = borrowId },
                Message = "Chuyen dat truoc thanh phieu muon thanh cong"
            });
        }
    }
}