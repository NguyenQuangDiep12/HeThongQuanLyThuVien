using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Publishers
{
    public class PublisherRequest { 
        [Required(ErrorMessage = "Tên nhà xuất bản không được để trống")] 
        public string PublisherName { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Logo không được để trống")]
        [Url(ErrorMessage = "LogoUrl không hợp lệ")] 
        public string LogoUrl { get; set; } = string.Empty; 
    }
}