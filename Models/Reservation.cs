using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.WAITING;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}