namespace HeThongQuanLyThuVien.DTOs.Statistics
{
    public class OverdueBorrowResponse
    {
        public int BorrowId { get; set; }

        public string ReaderName { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public int OverdueDays { get; set; }
    }
}
