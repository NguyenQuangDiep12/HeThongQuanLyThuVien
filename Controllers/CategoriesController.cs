using HeThongQuanLyThuVien.DTOs.Categories;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET /api/categories  (Public)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListCategories(CancellationToken ct)
        {
            var result = await _categoryService.GetListCategoriesAsync(ct);
            return Ok(new ApiResponse<List<CategoryResponse>>
            {
                Success = true,
                Data = result,
                Message = "Lay danh sach danh muc thanh cong"
            });
        }

        // GET /api/categories/:id  (Public)
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryDetail([FromRoute] int id, CancellationToken ct)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id, ct);
            return Ok(new ApiResponse<CategoryResponse>
            {
                Success = true,
                Data = result,
                Message = "Lay chi tiet danh muc thanh cong"
            });
        }

        // POST /api/categories  (Staff/Admin)
        [HttpPost]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
        {
            var result = await _categoryService.AddCategoryAsync(request, ct);
            return Ok(new ApiResponse<CategoryResponse>
            {
                Success = true,
                Data = result,
                Message = "Them danh muc thanh cong"
            });
        }

        // PUT /api/categories/:id  (Staff/Admin)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
        {
            await _categoryService.UpdateCategoryAsync(id, request, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cap nhat danh muc thanh cong"
            });
        }

        // DELETE /api/categories/:id  (Admin)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id, CancellationToken ct)
        {
            await _categoryService.DeleteCategoryAsync(id, ct);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Xoa danh muc thanh cong"
            });
        }
    }
}