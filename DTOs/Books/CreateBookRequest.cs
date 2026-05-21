namespace HeThongQuanLyThuVien.DTOs.Books
{
    public record CreateBookRequest(
        string Title, 
        string ISBN,
        int CategoryId,
        int PublisherId,
        int AuthorId,
        int Quantity,
        string Language,
        string Description,
        string CoverImage);
}
