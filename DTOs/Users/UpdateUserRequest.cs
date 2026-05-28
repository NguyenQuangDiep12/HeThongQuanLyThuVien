namespace HeThongQuanLyThuVien.DTOs.Users
{
    public record UpdateUserRequest(
        string FullName, 
        string Phone, 
        string Address, 
        int RoleId
        );
}
