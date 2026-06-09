using HeThongQuanLyThuVien.DTOs.Notifications;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// Notification chỉ phục vụ inbox của Reader.
    /// Workflow gia hạn (Pending/Approved/Rejected) được quản lý qua BorrowRecord.ExtensionRequestStatus.
    /// SendToStaffAsync đã bị xóa — không còn dùng Notification làm queue cho Staff.
    /// </summary>
    public interface INotificationService
    {
        // GET /notifications
        Task<PaginationResponse<NotificationResponse>> GetNotificationsAsync(
            int currentUserId,
            PaginationRequest request,
            CancellationToken ct = default);

        // GET /notifications/:id
        Task<NotificationResponse> GetNotificationByIdAsync(
            int notificationId,
            int currentUserId,
            CancellationToken ct = default);

        // PATCH /notifications/:id/read
        Task MarkAsReadAsync(
            int notificationId,
            int currentUserId,
            CancellationToken ct = default);

        // PATCH /notifications/read-all
        Task MarkAllAsReadAsync(
            int currentUserId,
            CancellationToken ct = default);

        /// <summary>
        /// Internal: Gửi thông báo đến một Reader cụ thể.
        /// Dùng sau khi duyệt gia hạn, từ chối gia hạn, tạo phiếu phạt, v.v.
        /// </summary>
        Task SendAsync(
            int userId,
            NotificationType type,
            string title,
            string content,
            CancellationToken ct = default);
    }
}