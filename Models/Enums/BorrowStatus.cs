namespace HeThongQuanLyThuVien.Models.Enums
{
    public enum BorrowStatus 
    { 
        PENDING, // Dang cho duoc muon
        BORROWING, // Dang duoc muon
        RETURNED, // Da duoc tra ve 
        OVERDUE, // Qua han
        CANCELLED // Huy Bo
    }
}
