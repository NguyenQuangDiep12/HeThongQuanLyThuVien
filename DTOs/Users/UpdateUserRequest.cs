using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    public class UpdateUserRequest 
    { 
        [Required]
        [StringLength(100, MinimumLength = 2)] 
        public string FullName { get; set; } = string.Empty; 
        [Required]
        [Phone] 
        public string Phone { get; set; } = string.Empty; 
        [Required] 
        public string Address { get; set; } = string.Empty; 
        [Range(1, int.MaxValue)] 
        public int RoleId { get; set; } 
    }
}

