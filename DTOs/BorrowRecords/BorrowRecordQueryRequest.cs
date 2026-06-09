using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    public class BorrowRecordQueryRequest : PaginationRequest
    {
        // Mã phiếu mượn
        public string? BorrowCode { get; set; }

        // Tên người mượn
        public string? ReaderName { get; set; }

        // Trạng thái phiếu mượn
        public BorrowStatus? Status { get; set; }

        /// <summary>
        /// Filter theo trạng thái yêu cầu gia hạn.
        /// Staff/Admin dùng ExtensionRequestStatus = PENDING để xem danh sách chờ duyệt.
        /// GET /borrow-records?extensionRequestStatus=PENDING
        /// </summary>
        public ExtensionRequestStatus? ExtensionRequestStatus { get; set; }
    }
}