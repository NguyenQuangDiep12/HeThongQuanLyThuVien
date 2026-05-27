namespace HeThongQuanLyThuVien.Models
{
    public class BookCategory
    {
        public int BookId { get; set; }
        public int CategoryId { get; set; }

        // Navigation
        public Book Book { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}