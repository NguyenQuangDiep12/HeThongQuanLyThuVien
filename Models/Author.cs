namespace HeThongQuanLyThuVien.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public string? AuthorUrl { get; set; }

        // Navigation
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}