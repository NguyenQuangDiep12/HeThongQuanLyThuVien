using System;

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

        private int _page = 1;

        public int Page
        {
            get => _page;
            set => _page = value <= 0 ? 1 : value;
        }

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                {
                    _pageSize = 10;
                }
                else
                {
                    _pageSize = value > MaxPageSize
                        ? MaxPageSize
                        : value;
                }
            }
        }
    }
}
