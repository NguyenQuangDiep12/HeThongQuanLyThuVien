using HeThongQuanLyThuVien.Models.Enums;
namespace HeThongQuanLyThuVien.DTOs.BorrowRecords { 
    public class BorrowDetailResponse { 
        public int BorrowDetailId { get; set; } 
        public int CopyId { get; set; }
        public string Barcode { get; set; } = string.Empty; 
        public string BookTitle { get; set; } = string.Empty; 
        public DateTime? ReturnedAt { get; set; } 
        public BookCondition? ItemCondition { get; set; } 
        public BorrowDetailStatus Status { get; set; } 
    }
}