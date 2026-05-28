using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/publishers")]
    public class PublishersController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListPublishers()
        {
            throw new NotImplementedException();
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublisherDetail()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> AddPublishers()
        {
            throw new NotImplementedException();
        }
        [HttpPut("{id}")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> UpdatePublishers()
        {
            throw new NotImplementedException();
        }
        [HttpDelete("{id}")]
        [Authorize("ADMIN")]
        public async Task<IActionResult> DeletePublishers()
        {
            throw new NotImplementedException();
        }
    }
}
