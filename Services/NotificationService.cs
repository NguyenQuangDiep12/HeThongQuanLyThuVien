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
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<NotificationResponse>> GetNotificationsAsync(int currentUserId,PaginationRequest request,CancellationToken ct = default)
        {
            var query = _context.Notifications.AsNoTracking().Where(n => n.UserId == currentUserId);
            int total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Content = n.Content,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                }).ToListAsync(ct);
            return new PaginationResponse<NotificationResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }
        public async Task<NotificationResponse> GetNotificationByIdAsync(int notificationId, int currentUserId,CancellationToken ct = default)
        {
            var notification = await _context.Notifications.AsNoTracking()
                .Where(n =>
                    n.NotificationId == notificationId &&
                    n.UserId == currentUserId)
                .Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Content = n.Content,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                }).FirstOrDefaultAsync(ct);
            if (notification is null)
                throw new NotFoundException("Thong bao khong ton tai!");
            return notification;
        }
        
        public async Task MarkAsReadAsync(int notificationId, int currentUserId, CancellationToken ct = default)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == currentUserId, ct);

            if (notification is null)
                throw new NotFoundException("Thong bao khong ton tai!");
            if (notification.IsRead)
                return;
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }

        public async Task MarkAllAsReadAsync(int currentUserId, CancellationToken ct = default)
        {
            await _context.Notifications
                .Where(n =>
                    n.UserId == currentUserId &&
                    !n.IsRead)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(n => n.IsRead, true)
                     .SetProperty(n => n.ReadAt,
                        DateTime.UtcNow), ct);
        }

        public async Task SendAsync(
            int userId,
            string title,
            string content,
            CancellationToken ct = default)
        {
            var notification = new Notification
            {
                UserId = userId,

                Title = title,

                Content = content,

                IsRead = false,

                CreatedAt = DateTime.UtcNow
            };

            await _context.Notifications
                .AddAsync(notification, ct);

            await _context.SaveChangesAsync(ct);
        }

        public async Task SendToStaffAsync(
            string title,
            string content,
            CancellationToken ct = default)
        {
            var staffIds = await _context.Users
                .Where(u => u.Role.RoleName == RoleName.STAFF)
                .Select(u => u.UserId)
                .ToListAsync(ct);

            var notifications = staffIds
                .Select(id => new Notification
                {
                    UserId = id,

                    Title = title,

                    Content = content,

                    IsRead = false,

                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            await _context.Notifications
                .AddRangeAsync(notifications, ct);

            await _context.SaveChangesAsync(ct);
        }
    }
}
