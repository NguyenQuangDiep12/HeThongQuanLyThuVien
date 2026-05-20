namespace HeThongQuanLyThuVien.DTOs.Authors
{
    public class UpdateAuthorRequest
    {
        public string AuthorName { get; set; } = string.Empty;

        public string Biography { get; set; } = string.Empty;

        public string AuthorUrl { get; set; } = string.Empty;
    }
}