namespace HeThongQuanLyThuVien.DTOs.Books
{
    public class CreateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        
        public string ISBN { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public int PublisherId { get; set; }

        public int Quantity { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
    }
}
