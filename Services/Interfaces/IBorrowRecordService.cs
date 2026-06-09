using HeThongQuanLyThuVien.DTOs.BorrowRecords;
using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC08  - Tạo phiếu mượn sách
    /// UC09  - Xem danh sách / lịch sử phiếu mượn
    /// UC10  - Xem chi tiết phiếu mượn
    /// UC11  - Gia hạn phiếu mượn
    /// UC12  - Hủy phiếu mượn
    /// UC13  - Xác nhận trả sách
    ///
    /// Quy tắc:
    /// - Tối đa 3 sách / lượt
    /// - Thời hạn mượn: 7 ngày
    /// - Gia hạn tối đa 2 lần
    /// - Mỗi lần gia hạn thêm 3 ngày
    ///
    /// Flow gia hạn:
    /// Reader gửi => BorrowRecord.ExtensionRequestStatus = PENDING
    /// Staff duyệt => APPROVED + Notification => Reader
    /// Staff từ chối => REJECTED + Notification => Reader
    /// </summary>
    public interface IBorrowRecordService
    {
        // GET /borrow-records — Staff/Admin xem danh sách
        Task<PaginationResponse<BorrowRecordSummaryResponse>> GetBorrowRecordsAsync(BorrowRecordQueryRequest request, CancellationToken ct = default);

        // GET /users/:id/borrow-records
        Task<PaginationResponse<BorrowRecordSummaryResponse>> GetUserBorrowRecordsAsync(int userId, int currentUserId, string currentRole, PaginationRequest request, CancellationToken ct = default);

        // GET /borrow-records/:id
        Task<BorrowRecordDetailResponse> GetBorrowRecordByIdAsync(int borrowId, int currentUserId, string currentRole, CancellationToken ct = default);

        // POST /borrow-records
        Task<BorrowRecordDetailResponse> CreateBorrowRecordAsync(int staffId, CreateBorrowRecordRequest request, CancellationToken ct = default);

        // PATCH /borrow-records/:id/return
        Task ConfirmReturnAsync(int borrowId, ConfirmReturnRequest request, CancellationToken ct = default);

        // PATCH /borrow-records/:id/cancel
        Task CancelBorrowRecordAsync(int borrowId, CancellationToken ct = default);

        /// <summary>
        /// Reader gửi yêu cầu gia hạn.
        /// Chỉ cập nhật ExtensionRequestStatus = PENDING trên BorrowRecord.
        /// Không tạo Notification, không gọi SendToStaffAsync.
        /// POST /borrow-records/:id/extension-requests
        /// </summary>
        Task SubmitExtensionRequestAsync(int borrowId, int readerId, CancellationToken ct = default);

        /// <summary>
        /// Staff/Admin duyệt yêu cầu gia hạn.
        /// Yêu cầu ExtensionRequestStatus == PENDING.
        /// Sau khi duyệt: ExtensionRequestStatus = APPROVED, gửi Notification cho Reader.
        /// Từ chối ExtensionRequestStatus = REJECTED, gửi notification cho Reader.
        /// PATCH /borrow-records/:id/extend
        /// </summary>
        Task ConfirmExtensionAsync(int borrowId, ProcessExtensionRequest request, CancellationToken ct = default);
    }
}