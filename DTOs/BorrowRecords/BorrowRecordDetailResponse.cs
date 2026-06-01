using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    public class BorrowRecordDetailResponse
    {
        public int BorrowId { get; set; }

        public int ReaderId { get; set; }
        public string ReaderName { get; set; } = null!;

        public int? ApproverId { get; set; }
        public string? ApproverName { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public BorrowStatus Status { get; set; }

        public List<BorrowDetailResponse> BorrowDetails { get; set; } = [];
    }
}
