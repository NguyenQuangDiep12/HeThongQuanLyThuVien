using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/borrow-records")]
    public class BorrowRecordsController : ControllerBase
    {
        [HttpGet]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> GetListBorrowRecords()
        {
            throw new NotImplementedException();
        }
        [HttpGet("/api/users/{id}/borrow-records")]
        [Authorize(Roles = "READER,STAFF,ADMIN")]
        public async Task<IActionResult> GetUserBorrowRecords(int id)
        {
            throw new NotImplementedException();
        }
        [HttpGet("{id}")]
        [Authorize("READER,STAFF,ADMIN")]
        public async Task<IActionResult> GetDetailBorrowRecord()
        {
            throw new NotImplementedException();
        }
        [HttpPost("{id}/extension-requests")]
        [Authorize("READER")]
        public async Task<IActionResult> SubmitBookRenewal()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> CreateBorrowRecord()
        {
            throw new NotImplementedException();
        }
        [HttpPatch("{id}/return")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> ConfirmBookReturned()
        {
            throw new NotImplementedException();
        }
        [HttpPatch("{id}/cancel")]
        [Authorize("READER,STAFF,ADMIN")]
        public async Task<IActionResult> CancelLoanPasses()
        {
            throw new NotImplementedException();
        }
        [HttpPatch("{id}/extend")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> ConfirmBookRenewal()
        {
            throw new NotImplementedException();
        }
    }
}
