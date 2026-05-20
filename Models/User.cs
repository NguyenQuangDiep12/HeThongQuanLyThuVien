using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string LibraryCardCode { get; set; } = string.Empty;
        public CardStatus CardStatus { get; set; } = CardStatus.PENDING;

        // Navigation
        public Role Role { get; set; } = null!;
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}