using HeThongQuanLyThuVien.DTOs.BookCopies;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/book-copies")]
    public class BookCopyController : ControllerBase
    {
        private readonly IBookCopyService _bookCopyService;

        public BookCopyController(IBookCopyService bookCopyService)
        {
            _bookCopyService = bookCopyService;
        }

        // GET /api/book-copies  (Staff/Admin)
        [HttpGet]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> GetBookCopies([FromQuery] GetBookCopiesRequest request, CancellationToken ct)
        {
            var result = await _bookCopyService.GetBookCopiesAsync(request, ct);
            return Ok(new ApiResponse<PaginationResponse<BookCopyResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach ban sao sach thanh cong"
            });
        }

        // GET /api/book-copies/:id  (Staff/Admin)
        [HttpGet("{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> GetBookCopyDetail([FromRoute] int id, CancellationToken ct)
        {
            var result = await _bookCopyService.GetBookCopyByIdAsync(id, ct);
            return Ok(new ApiResponse<BookCopyDetailResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet ban sao sach thanh cong"
            });
        }

        // POST /api/book-copies/book/:id  (Staff/Admin - them ban sao cho dau sach)
        [HttpPost("book/{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> AddBookCopy([FromRoute] int id, [FromBody] CreateBookCopyRequest request, CancellationToken ct)
        {
            var result = await _bookCopyService.CreateBookCopyAsync(id, request, ct);
            return Ok(new ApiResponse<CreateBookCopyResponse>
            {
                Success = true,
                Data = result,
                Message = "Them ban sao sach thanh cong"
            });
        }

        // PUT /api/book-copies/:id  (Staff/Admin - cap nhat toan bo thong tin ban sao)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> UpdateBookCopy([FromRoute] int id, [FromBody] UpdateBookCopyRequest request, CancellationToken ct)
        {
            await _bookCopyService.UpdateBookCopyAsync(id, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat ban sao sach thanh cong"
            });
        }

        // PATCH /api/book-copies/:id/status  (Staff/Admin - doi trang thai ban sao)
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> ChangeBookCopyStatus([FromRoute] int id, [FromBody] UpdateBookCopyStatusRequest request, CancellationToken ct)
        {
            await _bookCopyService.ChangeBookCopyStatusAsync(id, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat trang thai ban sao sach thanh cong"
            });
        }

        // DELETE /api/book-copies/:id  (Admin)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteBookCopy([FromRoute] int id, CancellationToken ct)
        {
            await _bookCopyService.DeleteBookCopyAsync(id, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Xoa ban sao sach thanh cong"
            });
        }
    }
}