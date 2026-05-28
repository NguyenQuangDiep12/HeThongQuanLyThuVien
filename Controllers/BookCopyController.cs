using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/book-copies")]
    public class BookCopyController : ControllerBase
    {
        [HttpGet]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> GetBookCopies()
        {
            throw new NotImplementedException();
        }
        [HttpGet("{id}")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> GetBookCopyDetail()
        {
            throw new NotImplementedException();
        }
        [HttpPost("book/{id}")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> AddBookCopyDetail()
        {
            throw new NotImplementedException();
        }
        [HttpPut("{id}")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> UpdateCopyBookStatus()
        {
            throw new NotImplementedException();
        }
        [HttpPatch("{id}/status")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> ChangeCopyBookStatus()
        {
            throw new NotImplementedException();
        }
        [HttpDelete("{id}")]
        [Authorize("ADMIN")]
        public async Task<IActionResult> DeleteBookCopy()
        {
            throw new NotImplementedException();
        }
    }
}
