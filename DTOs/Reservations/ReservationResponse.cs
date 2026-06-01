using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.DTOs.Reservations
{
    public record ReservationResponse(int ReservationId, int UserId, string FullName, int BookId, string Title, string Status, DateTime CreatedAt);
}
