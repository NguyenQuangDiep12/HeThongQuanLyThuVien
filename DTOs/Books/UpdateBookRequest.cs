namespace HeThongQuanLyThuVien.DTOs.Books
{
    public record UpdateBookRequest(
        string Title,
        string ISBN,
        int CategoryId,
        int AuthorId,
        int PublisherId);
}