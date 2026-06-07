namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    public class BookCopyDetailResponse
    {
        public int CopyId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string? ShelfLocation { get; set; }
        public string Status { get; set; } = string.Empty;  
        public string Condition { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }

        // Thong tin bo sung tu dau sach goc + Author + Publisher
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public List<string>? AuthorName { get; set; }
        public string? Isbn { get; set; }
        public string? Publisher { get; set; }
    }
}
