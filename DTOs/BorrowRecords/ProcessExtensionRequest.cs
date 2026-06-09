using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    public class ProcessExtensionRequest
    {
        [Required]
        public bool IsApproved { get; set; }

        // Bắt buộc khi IsApproved = false
        public string? Reason { get; set; }
    }
}
