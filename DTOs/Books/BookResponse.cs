namespace HeThongQuanLyThuVien.DTOs.Books
{
    // GET /books
    public record BookResponse(
        int BookId, 
        string Title, 
        string ISBN, 
        int Quantity, 
        int AvailableQuantity, 
        string Language, 
        string Description,  
        string CoverImage);
}
 
