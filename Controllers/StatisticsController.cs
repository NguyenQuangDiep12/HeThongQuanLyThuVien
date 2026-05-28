using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/statistic")]
    [Authorize("ADMIN")]
    public class StatisticsController : ControllerBase
    {
        [HttpGet("overviews")]
        public async Task<IActionResult> Overview()
        {
            throw new Exception();
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> Overdue()
        {
            throw new Exception();
        }
        [HttpGet("top-books")]
        public async Task<IActionResult> TopBooks()
        {
            throw new Exception();
        }
    }
}
