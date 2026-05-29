using HeThongQuanLyThuVien.DTOs.Statistics;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC25 - Thống kê báo cáo (Admin)
    ///   - Tổng số sách, người dùng, phiếu mượn
    ///   - Danh sách phiếu quá hạn
    ///   - Danh sách sách được mượn nhiều nhất
    /// </summary>
    public interface IStatisticsService
    {
        // GET /statistic/overviews
        Task<OverviewStatsResponse> GetOverviewAsync(CancellationToken ct = default);

        // GET /statistic/overdue
        Task<List<OverdueBorrowResponse>> GetOverdueRecordsAsync(CancellationToken ct = default);

        // GET /statistic/top-books
        Task<List<TopBookResponse>> GetTopBooksAsync(int topN = 10, CancellationToken ct = default);
    }
}