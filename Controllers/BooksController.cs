using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        [HttpGet("books")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooks()
        {
            throw new Exception();
        }

        [HttpGet("books/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookDetail()
        {
            throw new Exception();
        }
        [HttpPost("book")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> AddBookDetail()
        {
            throw new Exception();
        }
        [HttpPut("books/{id}")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> UpdateBookDetail()
        {
            throw new Exception();
        }
        [HttpDelete("book/{id}")]
        [Authorize("ADMIN")]
        public async Task<IActionResult> DeleteBookDetail()
        {
            throw new Exception();
        }
    }
}
