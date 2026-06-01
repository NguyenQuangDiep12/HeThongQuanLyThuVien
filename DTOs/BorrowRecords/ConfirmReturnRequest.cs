namespace HeThongQuanLyThuVien.DTOs.BorrowRecords
{
    // PATCH /borrow-records/:id/return
    // Staff truyen len tinh trang vat ly cua tung ban sao khi nhan lai
    // ReturnItems co the null hoac rong neu tat ca sach deu NORMAL
    public class ConfirmReturnRequest
    {
        public List<ReturnItemCondition>? ReturnItems { get; set; }
    }
}