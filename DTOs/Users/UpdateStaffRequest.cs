namespace HeThongQuanLyThuVien.DTOs.Users
{
    public record UpdateStaffRequest(
        // PUT /staff/:id
        string FullName, 
        string Phone, 
        string Address
        );
}
