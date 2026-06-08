namespace HeThongQuanLyThuVien.DTOs.Authors
{
    public record AuthorResponse(
        int AuthorId, 
        string AuthorName, 
        string? Biography, 
        string? AuthorUrl
    );
}