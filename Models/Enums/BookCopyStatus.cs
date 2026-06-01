namespace HeThongQuanLyThuVien.Models.Enums
{
    // trang thai nghiep vu cua cuon sach
    public enum BookCopyStatus 
    { 
        AVAILABLE, // co the muon, co san
        UNAVAILABLE, // khong co san, khong the muon
        BORROWED, // dang duoc muon
        RESERVED, // da duoc dat truoc boi nguoi khac
        LOST // mat vinh vien, khong con van hanh

    }
}
