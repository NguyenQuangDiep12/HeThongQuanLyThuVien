using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    // POST /staff
    public class CreateStaffRequest 
    { 
        [Required]
        [EmailAddress] 
        public string Email { get; set; } = string.Empty; 
        [Required]
        [StringLength(100, MinimumLength = 2)] 
        public string FullName { get; set; } = string.Empty; 
        [Required][MinLength(6)] 
        public string Password { get; set; } = string.Empty; 
        [Required]
        [Phone] 
        public string Phone { get; set; } = string.Empty; 
        [Required] 
        public string Address { get; set; } = string.Empty; 
    }
}
