using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Statistics;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /stats/overview — Thong ke tong quan
        public async Task<OverviewStatsResponse> GetOverviewAsync(CancellationToken ct = default)
        {
            var totalBooks = await _context.Books.CountAsync(ct);
            var totalUsers = await _context.Users
                .Where(u => u.Role.RoleName == RoleName.READER)
                .CountAsync(ct);
            var totalBorrowRecords = await _context.BorrowRecords.CountAsync(ct);

            var now = DateTime.UtcNow;
            var totalOverdue = await _context.BorrowRecords
                .Where(br =>
                    br.Status == BorrowStatus.Borrowing &&
                    br.DueDate < now)
                .CountAsync(ct);

            return new OverviewStatsResponse
            {
                TotalBooks = totalBooks,
                TotalUsers = totalUsers,
                TotalBorrowRecords = totalBorrowRecords,
                TotalOverdueRecords = totalOverdue
            };
        }

        // GET /stats/overdue — Danh sach phieu muon qua han
        public async Task<List<OverdueBorrowResponse>> GetOverdueRecordsAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            return await _context.BorrowRecords
                .AsNoTracking()
                .Include(br => br.Reader)
                .Where(br =>
                    br.Status == BorrowStatus.Borrowing &&
                    br.DueDate < now)
                .Select(br => new OverdueBorrowResponse
                {
                    BorrowId = br.BorrowId,
                    ReaderName = br.Reader.FullName,
                    DueDate = br.DueDate,
                    OverdueDays = (int)(now - br.DueDate).TotalDays
                })
                .OrderByDescending(x => x.OverdueDays)
                .ToListAsync(ct);
        }

        // GET /stats/top-books — Sach duoc muon nhieu nhat
        public async Task<List<TopBookResponse>> GetTopBooksAsync(int top = 10, CancellationToken ct = default)
        {
            return await _context.BorrowDetails
                .AsNoTracking()
                .GroupBy(bd => bd.BookCopy.BookId)
                .Select(g => new TopBookResponse
                {
                    BookId = g.Key,
                    Title = g.First().BookCopy.Book.Title,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(top)
                .ToListAsync(ct);
        }
    }
}