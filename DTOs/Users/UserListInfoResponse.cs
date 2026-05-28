namespace HeThongQuanLyThuVien.DTOs.Users
{
    public class UserListInfoResponse
    {
        // GET /users
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
        public string Phone { get; set; } = string.Empty; 
        public string Role { get; set; } = string.Empty; 
        public string Status { get; set; } = string.Empty; 
        public string CardStatus { get; set; } = string.Empty;
    }
}
