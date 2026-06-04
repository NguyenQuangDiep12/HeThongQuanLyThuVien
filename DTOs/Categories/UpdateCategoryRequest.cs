using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Categories
{
    public class UpdateCategoryRequest { 
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)] 
        public string CategoryName { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Mô tả không được để trống")] 
        public string Description { get; set; } = string.Empty; 
    }
}