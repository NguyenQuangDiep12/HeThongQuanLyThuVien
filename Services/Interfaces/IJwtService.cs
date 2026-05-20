using HeThongQuanLyThuVien.Models;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
