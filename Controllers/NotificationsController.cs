using HeThongQuanLyThuVien.DTOs.Notifications;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize(Roles = "READER")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET /api/notifications  (Reader xem danh sach thong bao cua chinh minh)
        [HttpGet]
        public async Task<IActionResult> GetListNotifications([FromQuery] PaginationRequest request, CancellationToken ct)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _notificationService.GetNotificationsAsync(currentUserId, request, ct);
            return Ok(new ApiResponse<PaginationResponse<NotificationResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach thong bao thanh cong"
            });
        }

        // GET /api/notifications/:id  (Reader xem chi tiet thong bao)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetNotificationDetail([FromRoute] int id, CancellationToken ct)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _notificationService.GetNotificationByIdAsync(id, currentUserId, ct);
            return Ok(new ApiResponse<NotificationResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet thong bao thanh cong"
            });
        }

        // PATCH /api/notifications/read-all  (Reader danh dau tat ca da doc)
        // Luu y: "read-all" phai dat TRUOC "{id:int}/read" de ASP.NET Core
        // khong bi ambiguous route khi parse literal "read-all" nham la int id
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllNotificationsSeen(CancellationToken ct)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _notificationService.MarkAllAsReadAsync(currentUserId, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Danh dau tat ca thong bao da doc thanh cong"
            });
        }

        // PATCH /api/notifications/:id/read  (Reader danh dau 1 thong bao cu the da doc)
        [HttpPatch("{id:int}/read")]
        public async Task<IActionResult> MarkNotificationAsRead([FromRoute] int id, CancellationToken ct)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _notificationService.MarkAsReadAsync(id, currentUserId, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Danh dau thong bao da doc thanh cong"
            });
        }
    }
}