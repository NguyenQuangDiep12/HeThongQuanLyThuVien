namespace HeThongQuanLyThuVien.DTOs.Shared
{
    /// <summary>
    /// GET /users => Danh sach nguoi dung (Staff/Admin)
    /// GET /users/:id/borrow-history => Lich su muon cua user (Reader/Staff/Admin)
    /// GET /books => tim kiem sach /books?categoryId=1 ,/books? authorId = 2 ,/ books ? language = English ,/books?keyword=harry&page=1&pageSize=5
    /// GET /borrow-records => danh sach phieu muon (Admin/Staff)
    /// GET /fines => danh sach phieu phat (Staff, Admin)
    /// GET /reservations => danh sach dat truoc (Staff/Admin)
    /// GET /notifications => thong bao toi user (READER)
    /// GET /notifications/read-all => doc tat ca thong bao (READER)
    /// </summary>
    public class PaginationRequest
    {
        private const int MaxPageSize = 50;
        public int Page {  get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
