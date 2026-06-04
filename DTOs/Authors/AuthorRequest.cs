using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Authors
{
    public class AuthorRequest { 
        [Required(ErrorMessage = "Tên tác giả không được để trống")]
        [StringLength(100)] public string AuthorName { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Tiểu sử không được để trống")]
        public string Biography { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Url không được để trống")]
        [Url(ErrorMessage = "Url không hợp lệ")] 
        public string AuthorUrl { get; set; } = string.Empty; 
    }
}