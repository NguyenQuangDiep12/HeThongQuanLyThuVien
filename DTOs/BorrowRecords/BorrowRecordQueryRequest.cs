using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    public class BorrowRecordQueryRequest : PaginationRequest
    {
        // Mã phiếu mượn
        public string? BorrowCode { get; set; }
        // Ten nguoi muon
        public string? ReaderName { get; set; }
        // Trang thai phieu
        public BorrowStatus? Status { get; set; }
    }
}