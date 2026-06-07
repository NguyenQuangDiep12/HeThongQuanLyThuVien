namespace HeThongQuanLyThuVien.Options
{
    public class LibrarySettings
    {
        public int MaxBooksPerBorrow { get; set; }
        public int BorrowDurationDays { get; set; }
        public int ExtensionDays { get; set; }
        public int MaxExtensions { get; set; }
        public decimal OverdueFinePerDay { get; set; }
    }
}
