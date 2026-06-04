using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Books
{
    public class CreateBookRequest
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phải chọn ít nhất một danh mục")]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất một danh mục")]
        public List<int> CategoryIds { get; set; } = [];

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PublisherId phải lớn hơn 0")]
        public int PublisherId { get; set; }

        [Required(ErrorMessage = "Phải chọn ít nhất một tác giả")]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất một tác giả")]
        public List<int> AuthorIds { get; set; } = [];

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Url]
        public string CoverImage { get; set; } = string.Empty;
    }
}