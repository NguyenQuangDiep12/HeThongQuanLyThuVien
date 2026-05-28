using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        [HttpGet]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> GetListResevations()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> CreatePreOrderBookForm()
        {
            throw new NotImplementedException();
        }
        [HttpPatch("{id}/cancel")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> CancelPreOrder()
        {
            throw new NotImplementedException();
        }
        [HttpPatch("{id}/complete")]
        [Authorize("STAFF,ADMIN")]
        public async Task<IActionResult> CompleteReservation()
        {
            throw new NotImplementedException();
        }
    }
}
