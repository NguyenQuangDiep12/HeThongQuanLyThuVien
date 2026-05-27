namespace HeThongQuanLyThuVien.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string from, string to, string subject, string body);
    }
}
