namespace HeThongQuanLyThuVien.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        // Navigation
        public User User { get; set; } = null!;
    }
}