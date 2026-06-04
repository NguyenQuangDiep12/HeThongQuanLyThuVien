using HeThongQuanLyThuVien.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    // PATCH /users/:id/card-status
    public class UpdateUserStatusRequest 
    { 
        [Required] 
        public UserStatus Status 
        { get; set; } 
    }
}
