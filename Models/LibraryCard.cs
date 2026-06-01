using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class LibraryCard
    {
        public int CardId { get; set; }
        public int UserId { get; set; }
        public string LibraryCardCode { get; set; } = string.Empty;
        public CardStatus Status { get; set; } = CardStatus.ACTIVE;
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        // Navigation
        public User User { get; set; } = null!;
    }
}
