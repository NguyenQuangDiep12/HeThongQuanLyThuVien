namespace HeThongQuanLyThuVien.DTOs.Shared
{
    public class PaginationResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize); // phan vung so trang 100 phan tu / 10 phan tu moi trang => 10 trang
        public int TotalRecords
        {
            get; set;
        }
        public List<T> Items { get; set; } = new List<T>();
    }
}
