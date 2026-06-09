using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.Notifications
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }

        /// <summary>
        /// Loại thông báo để client render icon / màu phù hợp.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}