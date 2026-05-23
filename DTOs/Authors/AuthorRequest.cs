namespace HeThongQuanLyThuVien.DTOs.Authors
{
    public record AuthorRequest(
        string AuthorName, 
        string Biography, 
        string AuthorUrl
    );
}