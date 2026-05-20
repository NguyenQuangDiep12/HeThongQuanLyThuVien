namespace HeThongQuanLyThuVien.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public int CategoryId { get; set; }
        public int PublisherId { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Category Category { get; set; } = null!;
        public Publisher Publisher { get; set; } = null!;
        public Author Author { get; set; } = null!;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<BorrowDetail> BorrowDetails { get; set; } = new List<BorrowDetail>();
    }
}