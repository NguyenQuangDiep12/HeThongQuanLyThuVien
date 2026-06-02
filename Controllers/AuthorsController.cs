using HeThongQuanLyThuVien.DTOs.Authors;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET /api/authors  (Public)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListAuthors(CancellationToken ct)
        {
            var result = await _authorService.GetListAuthorsAsync(ct);
            return Ok(new ApiResponse<List<AuthorResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach tac gia thanh cong"
            });
        }

        // GET /api/authors/:id  (Public)
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuthorDetail([FromRoute] int id, CancellationToken ct)
        {
            var result = await _authorService.GetAuthorByIdAsync(id, ct);
            return Ok(new ApiResponse<AuthorResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet tac gia thanh cong"
            });
        }

        // POST /api/authors  (Staff/Admin)
        [HttpPost]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorRequest request, CancellationToken ct)
        {
            var result = await _authorService.AddAuthorAsync(request, ct);
            return Ok(new ApiResponse<AuthorResponse>
            {
                Success = true,
                Data = result,
                Message = "Them tac gia thanh cong"
            });
        }

        // PUT /api/authors/:id  (Staff/Admin)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> UpdateAuthor([FromRoute] int id, [FromBody] AuthorRequest request, CancellationToken ct)
        {
            await _authorService.UpdateAuthorAsync(id, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat tac gia thanh cong"
            });
        }

        // DELETE /api/authors/:id  (Admin)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteAuthor([FromRoute] int id, CancellationToken ct)
        {
            await _authorService.DeleteAuthorAsync(id, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Xoa tac gia thanh cong"
            });
        }
    }
}