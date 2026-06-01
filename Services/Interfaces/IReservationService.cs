using HeThongQuanLyThuVien.DTOs.Reservations;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC23 - Đặt trước sách (Staff tạo khi sách không khả dụng)
    ///   - Khi sách khả dụng, chuyển phiếu đặt trước thành phiếu mượn
    ///   - Một người dùng chỉ được đặt trước một đầu sách một lần
    /// </summary>
    public interface IReservationService
    {
        // GET /reservations
        Task<PaginationResponse<ReservationResponse>> GetReservationsAsync(PaginationRequest request, CancellationToken ct = default);

        // POST /reservations — Staff tạo phiếu đặt trước cho bạn đọc
        Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request, CancellationToken ct = default);

        // PATCH /reservations/:id/cancel - READER,STAFF,ADMIN co the huy dat truoc sach
        Task CancelReservationAsync(int reservationId, CancellationToken ct = default);

        // PATCH /reservations/:id/complete — chuyển thành phiếu mượn khi sách khả dụng
        Task<int> CompleteReservationAsync(int reservationId, int staffId, CancellationToken ct = default);
    }
}