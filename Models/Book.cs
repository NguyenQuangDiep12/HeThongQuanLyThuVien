namespace HeThongQuanLyThuVien.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public int PublisherId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImage { get; set; }
        public int AvailabilityCopies { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Publisher Publisher { get; set; } = null!;

        // Explicit junction — dung khi can truy cap truc tiep bang trung gian
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

        public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}