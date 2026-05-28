using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListCategories()
        {
            throw new NotImplementedException();
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryDetail()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> AddCategories()
        {
            throw new NotImplementedException();
        }
        [HttpPut("{id}")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> UpdateCategories()
        {
            throw new NotImplementedException();
        }
        [HttpDelete("{id}")]
        [Authorize("ADMIN")]
        public async Task<IActionResult> DeleteCategories()
        {
            throw new NotImplementedException();
        }
    }
}
