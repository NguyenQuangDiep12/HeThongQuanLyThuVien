using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> GetUsers()
        {
            throw new Exception();
        }

        [HttpGet("{id}")]
        [Authorize("READER,STAFF,ADMIN")]
        public async Task<IActionResult> GetUserDetail()
        {
            throw new Exception();
        }
        [HttpPut("{id}")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> UpdateUserInfo()
        {
            throw new Exception();
        }
        [HttpPut("me/profile")]
        [Authorize("READER")]
        public async Task<IActionResult> UpdateMyProfile()
        {
            throw new Exception();
        }
        [HttpPatch("{id}/card-status")]
        [Authorize("ADMIN")]
        public async Task<IActionResult> UpdateCardStatus()
        {
            throw new Exception();
        }
        [HttpPatch("{id}/status")]
        [Authorize("ADMIN")]
        public async Task<IActionResult> UpdateUserStatus()
        {
            throw new Exception();
        }
        [HttpPost("staff")]
        [Authorize("ADMIN")]
        public async Task<IActionResult> CreateStaff()
        {
            throw new Exception();
        }
        [HttpGet("staffs")]
        [Authorize("ADMIN")]
        public async Task<IActionResult> GetStaffs()
        {
            throw new Exception();
        }
    }
}
