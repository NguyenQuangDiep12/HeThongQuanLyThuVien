namespace HeThongQuanLyThuVien.Models
{
    public class Publisher
    {
        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;

        // Navigation
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}