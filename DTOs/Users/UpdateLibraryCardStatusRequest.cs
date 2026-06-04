using HeThongQuanLyThuVien.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Users
{
    public class UpdateLibraryCardStatusRequest 
    { 
        [Required] 
        public CardStatus Status { get; set; } 
    }
}
