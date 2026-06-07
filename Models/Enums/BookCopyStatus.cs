namespace HeThongQuanLyThuVien.Models.Enums
{
    // trang thai nghiep vu cua cuon sach
    // Cuốn sách này hiện có thể được mượn không?
    public enum BookCopyStatus
    {
        AVAILABLE,    // Có thể mượn
        BORROWED,     // Đang được mượn
        RESERVED,     // Đã được đặt trước
        UNAVAILABLE   // Tạm ngưng lưu hành (chờ staff kiểm tra)
    }
}
