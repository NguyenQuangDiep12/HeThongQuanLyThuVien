using HeThongQuanLyThuVien.DTOs.Shared;

namespace HeThongQuanLyThuVien.DTOs.Books
{
    public class BookQueryRequest : PaginationRequest
    {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
    }
}
