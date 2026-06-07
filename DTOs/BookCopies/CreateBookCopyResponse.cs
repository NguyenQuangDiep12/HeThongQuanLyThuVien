namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    public class CreateBookCopyResponse
    {
        public int TotalCreated { get; set; }
        public List<BookCopyResponse> Copies { get; set; } = new List<BookCopyResponse>();
    }
}
