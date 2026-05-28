using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        [HttpGet]
        [Authorize("READER")]
        public async Task<IActionResult> GetListNotifications()
        {
            throw new NotImplementedException();
        }
        [HttpPatch("read-all")]
        [Authorize("READER")]
        public async Task<IActionResult> MarkAllNotificationsSeen()
        {
            throw new NotImplementedException();
        }
    }
}
