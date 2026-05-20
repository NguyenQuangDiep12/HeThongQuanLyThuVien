using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public RoleName RoleName { get; set; } = RoleName.READER;
        public string Description { get; set; } = string.Empty;

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}