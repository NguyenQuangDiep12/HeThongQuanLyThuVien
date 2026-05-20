namespace HeThongQuanLyThuVien.DTOs.Statistics
{
    public class TopBookResponse
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int BorrowCount { get; set; }
    }
}
