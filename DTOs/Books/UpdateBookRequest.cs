using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.Books
{
    public class UpdateBookRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<int> CategoryIds { get; set; } = [];

        [Required]
        [MinLength(1)]
        public List<int> AuthorIds { get; set; } = [];

        [Range(1, int.MaxValue)]
        public int PublisherId { get; set; }

        public string? Language { get; set; }

        public string? Description { get; set; }

        public string? CoverImage { get; set; }
    }
}