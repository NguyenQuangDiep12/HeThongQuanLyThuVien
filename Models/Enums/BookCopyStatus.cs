namespace HeThongQuanLyThuVien.Models.Enums
{
    // trang thai nghiep vu cua cuon sach
    public enum BookCopyStatus
    {
        AVAILABLE,    // Có thể mượn
        BORROWED,     // Đang được mượn
        RESERVED,     // Đã được đặt trước
        DAMAGED,      // Hỏng (theo ERD/UC17)
        LOST,         // Mất vĩnh viễn
        UNAVAILABLE   // Tạm ngưng lưu hành (chờ staff kiểm tra)
    }
}
