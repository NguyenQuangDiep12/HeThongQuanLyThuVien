using HeThongQuanLyThuVien.DTOs.Fines;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC14 - Tạo phiếu phạt (Staff)
    ///   Loại vi phạm: Trả quá hạn, Làm hỏng sách, Làm mất sách
    /// UC15 - Thanh toán phiếu phạt (Staff xác nhận)
    /// </summary>
    public interface IFineService
    {
        // GET /fines
        Task<PaginationResponse<FineResponse>> GetListFinesAsync(
            int page, int pageSize, CancellationToken ct = default);

        // GET /fines/:id
        Task<FineDetailResponse> GetFineByIdAsync(int fineId, CancellationToken ct = default);

        // GET /users/:id/fines — theo dõi toàn bộ vi phạm của một người dùng
        Task<PaginationResponse<FineResponse>> GetUserFinesAsync(
            int userId, int page, int pageSize, CancellationToken ct = default);

        // UC14 — POST /fines
        Task<FineDetailResponse> CreateFineAsync(CreateFineRequest request, CancellationToken ct = default);

        // UC15 — PATCH /fines/:id/pay
        Task ConfirmPaymentAsync(int fineId, CancellationToken ct = default);
    }
}