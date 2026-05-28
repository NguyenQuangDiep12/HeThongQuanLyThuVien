using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/fines")]
    public class FinesController : ControllerBase
    {
        [HttpGet]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> GetListFines()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> CreateFineTicket()
        {
            throw new Exception();
        }
        [HttpGet("/api/users/{id}/fines")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> MonitorUserViolations()
        {
            throw new NotImplementedException();
        }
        [HttpGet("{id}")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> GetDetailFine()
        {
            throw new NotImplementedException();
        }
        [HttpPatch("{id}/pay")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> PaymentConfirmation()
        {
            throw new NotImplementedException();
        }
    }
}
