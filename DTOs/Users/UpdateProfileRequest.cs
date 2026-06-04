using HeThongQuanLyThuVien.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    // PUT /users/me/profile
    public class UpdateProfileRequest 
    { 
        [Required]
        [StringLength(100, MinimumLength = 2)] 
        public string FullName { get; set; } = string.Empty; 

        [Required]
        [Phone] 
        public string Phone { get; set; } = string.Empty; 

        [Required] 
        public string Address { get; set; } = string.Empty; 

        [Url(ErrorMessage = "AvatarUrl không hợp lệ")] 
        public string? AvatarUrl { get; set; } 
    }
}
