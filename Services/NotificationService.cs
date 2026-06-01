using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Notifications;
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

        // GET /notifications — Reader lay danh sach thong bao (UC24)
        //public async Task<List<NotificationResponse>> GetNotificationsAsync(int userId, CancellationToken ct = default)
        //{
        //    return await _context.Notifications
        //        .AsNoTracking()
        //        .Where(n => n.UserId == userId)
        //        .OrderByDescending(n => n.CreatedAt)
        //        .Select(n => new NotificationResponse
        //        {
        //            NotificationId = n.NotificationId,
        //            Title = n.Title,
        //            Content = n.Content,
        //            IsRead = n.IsRead,
        //            ReadAt = n.ReadAt,
        //            CreatedAt = n.CreatedAt
        //        })
        //        .ToListAsync(ct);
        //}

        // PATCH /notifications/read-all — Danh dau tat ca da doc (UC24)
        public async Task MarkAllAsReadAsync(int userId, CancellationToken ct = default)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(n => n.IsRead, true)
                     .SetProperty(n => n.ReadAt, DateTime.UtcNow), ct);
        }

        // Internal: gui thong bao cho nguoi dung cu the
        public async Task SendAsync(int userId, string title, string content, CancellationToken ct = default)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Content = content,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification, ct);
            await _context.SaveChangesAsync(ct);
        }

        // Internal: gui thong bao cho tat ca Staff
        public async Task SendToStaffAsync(string message, CancellationToken ct = default)
        {
            var staffIds = await _context.Users
                .Where(u => u.Role.RoleName == RoleName.STAFF)
                .Select(u => u.UserId)
                .ToListAsync(ct);

            var notifications = staffIds.Select(id => new Notification
            {
                UserId = id,
                Title = "Yeu cau gia han sach",
                Content = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Notifications.AddRangeAsync(notifications, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}