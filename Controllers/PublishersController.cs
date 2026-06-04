using HeThongQuanLyThuVien.DTOs.Publishers;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/publishers")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        // GET /api/publishers  (Public)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListPublishers(CancellationToken ct)
        {
            var result = await _publisherService.GetListPublishersAsync(ct);
            return Ok(new ApiResponse<List<PublisherResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach nha xuat ban thanh cong"
            });
        }

        // GET /api/publishers/:id  (Public)
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublisherDetail([FromRoute] int id, CancellationToken ct)
        {
            var result = await _publisherService.GetPublisherByIdAsync(id, ct);
            return Ok(new ApiResponse<PublisherResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet nha xuat ban thanh cong"
            });
        }

        // POST /api/publishers  (Staff/Admin)
        [HttpPost]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> AddPublisher([FromBody] PublisherRequest request, CancellationToken ct)
        {
            var result = await _publisherService.AddPublisherAsync(request, ct);
            return Ok(new ApiResponse<PublisherResponse>
            {
                Success = true,
                Data = result,
                Message = "Them nha xuat ban thanh cong"
            });
        }

        // PUT /api/publishers/:id  (Staff/Admin)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> UpdatePublisher([FromRoute] int id, [FromBody] PublisherRequest request, CancellationToken ct)
        {
            await _publisherService.UpdatePublisherAsync(id, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat nha xuat ban thanh cong"
            });
        }

        // DELETE /api/publishers/:id  (Admin)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeletePublisher([FromRoute] int id, CancellationToken ct)
        {
            await _publisherService.DeletePublisherAsync(id, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Xoa nha xuat ban thanh cong"
            });
        }
    }
}