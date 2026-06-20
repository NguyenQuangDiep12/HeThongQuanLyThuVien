using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.DTOs.Statistics;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/statistic")]
    [Authorize(Roles = "ADMIN")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        // GET /api/statistic/overviews  (Admin - thong ke tong quan)
        [HttpGet("overviews")]
        public async Task<IActionResult> Overview(CancellationToken ct)
        {
            var result = await _statisticsService.GetOverviewAsync(ct);
            return Ok(new ApiResponse<OverviewStatsResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay thong ke tong quan thanh cong"
            });
        }

        // GET /api/statistic/overdue  (Admin - danh sach phieu muon qua han)
        [HttpGet("overdue")]
        public async Task<IActionResult> Overdue(CancellationToken ct)
        {
            var result = await _statisticsService.GetOverdueRecordsAsync(ct);
            return Ok(new ApiResponse<List<OverdueBorrowResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach phieu muon qua han thanh cong"
            });
        }

        // GET /api/statistic/top-books  (Admin - sach duoc muon nhieu nhat)
        [HttpGet("top-books")]
        public async Task<IActionResult> TopBooks([FromQuery] int top, CancellationToken ct = default)
        {
            var result = await _statisticsService.GetTopBooksAsync(top, ct);
            return Ok(new ApiResponse<List<TopBookResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach sach duoc muon nhieu nhat thanh cong"
            });
        }
    }
}