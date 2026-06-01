using HeThongQuanLyThuVien.DTOs.Notifications;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC24 - Quản lý thông báo (Reader)
    ///   Loại thông báo: Quá hạn sách, Gia hạn thành công, Trả sách thành công
    /// </summary>
    public interface INotificationService
    {
        // GET /notifications — lấy danh sách thông báo của Reader hiện tại
        Task<PaginationResponse<NotificationResponse>> GetNotificationsAsync(
            int currentUserId, int page, int pageSize, CancellationToken ct = default);

        // PATCH /notifications/read-all — đánh dấu tất cả là đã đọc
        Task MarkAllAsReadAsync(int currentUserId, CancellationToken ct = default);

        // Internal gui thong bao cho nguoi dung cu the
        Task SendAsync(int userId, string title, string content, CancellationToken ct = default);

        // Internal gui thong bao den tat ca staff
        Task SendToStaffAsync(string message, CancellationToken ct = default);
    }
}