namespace HeThongQuanLyThuVien.DTOs.Books
{
    // GET /books
    public record BookResponse(
        int BookId,
        string Title,
        string ISBN,

        // Tong so ban sao
        int TotalCopies,

        // So ban sao co the muon
        int AvailableCopies,

        string Language,
        string Description,
        string CoverImage
    );
}
 
