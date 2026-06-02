using HeThongQuanLyThuVien.DTOs.Books;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET /api/books  (Public)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooks([FromQuery] BookQueryRequest request, CancellationToken ct)
        {
            var result = await _bookService.GetRangeBooksAsync(request, ct);
            return Ok(new ApiResponse<PaginationResponse<BookResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach sach thanh cong"
            });
        }

        // GET /api/books/:id  (Public)
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookDetail([FromRoute] int id, CancellationToken ct)
        {
            var result = await _bookService.GetBookByIdAsync(id, ct);
            return Ok(new ApiResponse<BookDetailResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet sach thanh cong"
            });
        }

        // POST /api/books  (Staff/Admin)
        [HttpPost]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> AddBook([FromBody] CreateBookRequest request, CancellationToken ct)
        {
            var result = await _bookService.CreateBookAsync(request, ct);
            return Ok(new ApiResponse<BookResponse>
            {
                Success = true,
                Data = result,
                Message = "Them sach moi thanh cong"
            });
        }

        // PUT /api/books/:id  (Staff/Admin)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] UpdateBookRequest request, CancellationToken ct)
        {
            var result = await _bookService.UpdateBookAsync(id, request, ct);
            return Ok(new ApiResponse<BookResponse>
            {
                Success = true,
                Data = result,
                Message = "Cap nhat thong tin sach thanh cong"
            });
        }

        // DELETE /api/books/:id  (Admin)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteBook([FromRoute] int id, CancellationToken ct)
        {
            await _bookService.DeleteBookAsync(id, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Xoa sach thanh cong"
            });
        }
    }
}