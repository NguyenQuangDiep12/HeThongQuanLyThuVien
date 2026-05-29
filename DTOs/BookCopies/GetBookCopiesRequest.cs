using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.BookCopies
{
    public class GetBookCopiesRequest : PaginationRequest
    {
        public int? BookId { get; set; }
        public BookCopyStatus? Status { get; set; }
    }
}
