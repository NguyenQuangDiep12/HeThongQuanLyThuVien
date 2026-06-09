using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Notifications;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    /// <summary>
    /// Notification chỉ là inbox của Reader.
    /// SendToStaffAsync đã bị xóa — workflow gia hạn dùng BorrowRecord.ExtensionRequestStatus.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /notifications
        public async Task<PaginationResponse<NotificationResponse>> GetNotificationsAsync(
            int currentUserId,
            PaginationRequest request,
            CancellationToken ct = default)
        {
            var query = _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == currentUserId);

            int total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    Type = n.Type.ToString(),
                    Title = n.Title,
                    Content = n.Content,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync(ct);

            return new PaginationResponse<NotificationResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }

        // GET /notifications/:id
        public async Task<NotificationResponse> GetNotificationByIdAsync(
            int notificationId,
            int currentUserId,
            CancellationToken ct = default)
        {
            var notification = await _context.Notifications
                .AsNoTracking()
                .Where(n => n.NotificationId == notificationId && n.UserId == currentUserId)
                .Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    Type = n.Type.ToString(),
                    Title = n.Title,
                    Content = n.Content,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (notification is null)
                throw new NotFoundException("Thông báo không tồn tại!");

            return notification;
        }

        // PATCH /notifications/:id/read
        public async Task MarkAsReadAsync(
            int notificationId,
            int currentUserId,
            CancellationToken ct = default)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == currentUserId, ct);

            if (notification is null)
                throw new NotFoundException("Thông báo không tồn tại!");

            if (notification.IsRead)
                return;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }

        // PATCH /notifications/read-all
        public async Task MarkAllAsReadAsync(int currentUserId, CancellationToken ct = default)
        {
            await _context.Notifications
                .Where(n => n.UserId == currentUserId && !n.IsRead)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(n => n.IsRead, true)
                     .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                    ct);
        }

        /// <summary>
        /// Internal: Gửi thông báo đến một Reader cụ thể.
        /// Caller truyền vào Type để phân loại thông báo.
        /// </summary>
        public async Task SendAsync(
            int userId,
            NotificationType type,
            string title,
            string content,
            CancellationToken ct = default)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Content = content,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}