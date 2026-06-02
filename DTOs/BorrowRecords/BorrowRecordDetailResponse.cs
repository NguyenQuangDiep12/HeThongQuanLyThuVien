using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    public class BorrowRecordDetailResponse
    {
        public int BorrowId { get; set; }

        public string BorrowCode { get; set; } = string.Empty;

        public int ReaderId { get; set; }
        public string ReaderName { get; set; } = string.Empty;

        public int? ApproverId { get; set; }
        public string? ApproverName { get; set; }

        public DateTime? BorrowDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public BorrowType BorrowType { get; set; }

        public BorrowStatus Status { get; set; }

        public int ExtensionCount { get; set; }

        public List<BorrowDetailResponse> BorrowDetails { get; set; } = [];
    }
}