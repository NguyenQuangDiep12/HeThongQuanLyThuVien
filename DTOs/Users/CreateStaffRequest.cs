namespace HeThongQuanLyThuVien.DTOs.Users
{
    // POST /staff
    public record CreateStaffRequest(
        string Email, 
        string FullName, 
        string Password, 
        string Phone, 
        string Address
    );
}
