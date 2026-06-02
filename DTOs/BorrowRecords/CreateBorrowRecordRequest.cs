using HeThongQuanLyThuVien.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    public class CreateBorrowRecordRequest
    {
        public int ReaderId { get; set; }
        public List<int> CopyIds { get; set; } = new();

        public BorrowType BorrowType { get; set; }
    }
}
