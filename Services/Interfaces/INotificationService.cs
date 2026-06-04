using HeThongQuanLyThuVien.DTOs.Notifications;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC24 - Quan ly thong bao (Reader)
    ///   Loai thong bao: Qua han sach, Gia han thanh cong, Tra sach thanh cong
    /// </summary>
    public interface INotificationService
    {
        // GET /notifications — lay danh sach thong bao cua Reader hien tai
        Task<PaginationResponse<NotificationResponse>> GetNotificationsAsync(int currentUserId, PaginationRequest request, CancellationToken ct = default);
        // GET /notifications/:id — lay chi tiet thong bao
        Task<NotificationResponse> GetNotificationByIdAsync(int notificationId, int currentUserId, CancellationToken ct = default);
        // PATCH /notifications/:id/read — danh dau 1 thong bao da doc
        Task MarkAsReadAsync(int notificationId, int currentUserId, CancellationToken ct = default);
        // PATCH /notifications/read-all — danh dau tat ca la da doc
        Task MarkAllAsReadAsync(int currentUserId, CancellationToken ct = default);
        // Internal: gui thong bao cho nguoi dung cu the
        Task SendAsync(int userId, string title, string content, CancellationToken ct = default);
        // Internal: gui thong bao den tat ca staff
        Task SendToStaffAsync(string title, string content, CancellationToken ct = default);
    }
}