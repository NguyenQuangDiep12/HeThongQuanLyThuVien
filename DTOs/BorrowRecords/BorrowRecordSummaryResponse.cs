namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    public class BorrowRecordSummaryResponse
    {
        public int BorrowId { get; set; }
        public string BorrowCode { get; set; } = string.Empty;
        public int ReaderId { get; set; }
        public string ReaderName { get; set; } = string.Empty;
        public DateTime? BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public int ExtensionCount { get; set; }
        public string BorrowType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái yêu cầu gia hạn hiện tại.
        /// Staff dùng để biết phiếu nào đang chờ duyệt.
        /// </summary>
        public string ExtensionRequestStatus { get; set; } = string.Empty;

        // Số lượng sách trong phiếu
        public int TotalBooks { get; set; }
    }
}