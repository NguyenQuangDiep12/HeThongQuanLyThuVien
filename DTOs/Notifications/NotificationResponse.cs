namespace HeThongQuanLyThuVien.DTOs.Notifications
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
