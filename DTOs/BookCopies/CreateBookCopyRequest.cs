namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    // CreateBookCopyRequest:
    public class CreateBookCopyRequest
    {
        public string Barcode { get; set; } = string.Empty;
        public string? ShelfLocation { get; set; }
        public bool IsReferenceOnly { get; set; } = false;
    }
}
