namespace HeThongQuanLyThuVien.DTOs.Authors
{
    public class AuthorResponse
    {
        public int AuthorId { get; set; }

        public string AuthorName { get; set; } = string.Empty;

        public string Biography { get; set; } = string.Empty;

        public string AuthorUrl { get; set; } = string.Empty;
    }
}