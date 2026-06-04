using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Reservations
{
    public class CreateReservationRequest { 
        [Range(1, int.MaxValue)] 
        public int UserId { get; set; } 
        [Range(1, int.MaxValue)] 
        public int BookId { get; set; } 
    }
}
