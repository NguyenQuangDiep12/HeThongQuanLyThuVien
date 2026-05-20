namespace HeThongQuanLyThuVien.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public string AuthorUrl { get; set; } = string.Empty;

        // Navigation
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}