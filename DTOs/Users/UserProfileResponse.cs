namespace HeThongQuanLyThuVien.DTOs.Users
{
    // GET /user/:id => xem chi tiet bo thong tin nguoi dung
    public class UserProfileResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
        public string Phone { get; set; } = string.Empty; 
        public string Address { get; set; } = string.Empty; 
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = string.Empty; 
        public string Status { get; set; } = string.Empty; 
        public string LibraryCardCode { get; set; } = string.Empty;
        public string CardStatus { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
    }
}
