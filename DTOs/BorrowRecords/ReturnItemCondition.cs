using HeThongQuanLyThuVien.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    // Tinh trang vat ly cua 1 ban sao cu the khi tra
    public class ReturnItemCondition
    {
        // CopyId de xac dinh chinh xac ban sao nao trong phieu muon
        [Range(1, int.MaxValue)]
        public int CopyId { get; set; }

        // Tinh trang vat ly Staff kiem tra khi nhan lai sach
        // Mac dinh NORMAL neu khong truyen
        public BookCondition Condition { get; set; } = BookCondition.NORMAL;
        public BookCopyStatus CopyStatus { get; set; } = BookCopyStatus.AVAILABLE;
    }
}
