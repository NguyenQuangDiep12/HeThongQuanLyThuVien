using HeThongQuanLyThuVien.DTOs.BorrowRecords;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC08  - Tạo phiếu mượn sách (Staff)
    /// UC09  - Xem lịch sử mượn sách
    /// UC10  - Xem chi tiết phiếu mượn
    /// UC11  - Gia hạn sách (Reader gửi yêu cầu / Staff xác nhận)
    /// UC12  - Hủy phiếu mượn
    /// UC13  - Xác nhận trả sách (Staff)
    ///
    /// Quy tắc nghiệp vụ:
    ///   - Mượn tối đa 3 sách / lượt, thời hạn 7 ngày
    ///   - Gia hạn tối đa 2 lần, mỗi lần +3 ngày
    /// </summary>
    public interface IBorrowRecordService
    {
        // UC09 (Staff/Admin) — GET /borrow-records
        Task<PaginationResponse<BorrowRecordResponse>> GetListBorrowRecordsAsync(
            int page, int pageSize, CancellationToken ct = default);

        // UC09 (Owner/Staff/Admin) — GET /users/:id/borrow-records
        Task<PaginationResponse<BorrowRecordResponse>> GetUserBorrowRecordsAsync(
            int userId, int page, int pageSize, CancellationToken ct = default);

        // UC10 — GET /borrow-records/:id
        Task<BorrowRecordDetailResponse> GetBorrowRecordByIdAsync(int borrowId, CancellationToken ct = default);

        // UC08 — POST /borrow-records  (Staff tạo phiếu mượn)
        Task<BorrowRecordDetailResponse> CreateBorrowRecordAsync(
            CreateBorrowRecordRequest request, CancellationToken ct = default);

        // UC11 bước 1 — POST /borrow-records/:id/extension-requests  (Reader gửi yêu cầu gia hạn)
        Task SubmitExtensionRequestAsync(int borrowId, int currentUserId, CancellationToken ct = default);

        // UC11 bước 2 — PATCH /borrow-records/:id/extend  (Staff xác nhận gia hạn)
        Task ConfirmExtensionAsync(int borrowId, CancellationToken ct = default);

        // UC13 — PATCH /borrow-records/:id/return  (Staff xác nhận trả sách)
        Task ConfirmReturnAsync(int borrowId, ConfirmReturnRequest request, CancellationToken ct = default);

        // UC12 — PATCH /borrow-records/:id/cancel
        Task CancelBorrowRecordAsync(int borrowId, int currentUserId, CancellationToken ct = default);
    }
}