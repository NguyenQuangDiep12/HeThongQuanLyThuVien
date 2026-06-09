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
    /// </summary>
    public interface IBorrowRecordService
    {
        /// <summary>
        /// Staff/Admin xem danh sách phiếu mượn
        /// GET /borrow-records
        /// </summary>
        Task<PaginationResponse<BorrowRecordSummaryResponse>> GetBorrowRecordsAsync(BorrowRecordQueryRequest request, CancellationToken ct = default);
        /// <summary>
        /// Reader xem lịch sử mượn của chính mình
        /// Staff/Admin xem lịch sử của user bất kỳ
        /// GET /users/:id/borrow-records
        /// </summary>
        Task<PaginationResponse<BorrowRecordSummaryResponse>> GetUserBorrowRecordsAsync(int userId, int currentUserId, string currentRole, PaginationRequest request, CancellationToken ct = default);
        /// <summary>
        /// Xem chi tiết phiếu mượn
        /// GET /borrow-records/:id
        /// </summary>
        Task<BorrowRecordDetailResponse> GetBorrowRecordByIdAsync(int borrowId, int currentUserId, string currentRole,CancellationToken ct = default);
        /// <summary>
        /// Staff tạo phiếu mượn
        /// POST /borrow-records
        /// </summary>
        Task<BorrowRecordDetailResponse> CreateBorrowRecordAsync(int staffId, CreateBorrowRecordRequest request, CancellationToken ct = default);

        /// <summary>
        /// Reader gửi yêu cầu gia hạn
        /// POST /borrow-records/:id/extension-requests
        /// </summary>
        Task SubmitExtensionRequestAsync(int borrowId, int readerId, CancellationToken ct = default);

        /// <summary>
        /// Staff xác nhận gia hạn
        /// PATCH /borrow-records/:id/extend
        /// </summary>
        Task ConfirmExtensionAsync(int borrowId, int staffId, CancellationToken ct = default);

        /// <summary>
        /// Staff/Admin xác nhận trả sách
        /// PATCH /borrow-records/:id/return
        /// </summary>
        Task ConfirmReturnAsync(int borrowId, ConfirmReturnRequest request, CancellationToken ct = default);

        /// <summary>
        /// Hủy phiếu mượn
        /// PATCH /borrow-records/:id/cancel
        /// </summary>
        Task CancelBorrowRecordAsync(int borrowId,CancellationToken ct = default);
    }
}