using HeThongQuanLyThuVien.Services.Interfaces;
using System.Net; // thu vien xu ly network
using System.Net.Mail; // thu vien gui mail co san trong .net

namespace HeThongQuanLyThuVien.Services
{
    public class MailService : IMailService
    {
        // ham nhan 3 tham so to(email nguoi nhan), from (nguoi gui), subject( tieu de), body (noi dung) chay bat dong bo async task
        public async Task SendEmailAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(); // MailMessage() dai dien cho mot mail

            message.From = new MailAddress($"{from}"); // gan dia chi nguoi gui
            message.To.Add(to); // gan dia chi nguoi nhan
            message.Subject = subject; // gan tieu de email
            message.Body = body; // gan noi dung email
            message.IsBodyHtml = false; // khong ho tro HTML

            // ket noi toi server cua gmail xem them https://stackoverflow.com/questions/63720377/sending-an-email-using-smtp-client
            using var smtp = new SmtpClient("smtp.gmail.com", 587); // "smtp.gmail.com" SMTP cua server gmail, "587" cong gui mail TLS 


            // ma xac thuc tinh hop le cho tai khoan gmail: nguyenquangdiepnx1@gmail.com
            string AppPasswordVerified = "brht gbor pzwr ueui";

            smtp.Credentials = new NetworkCredential(
                "nguyenquangdiepnx1@gmail.com",
                $"{AppPasswordVerified}"
                );

            smtp.EnableSsl = true;

            await smtp.SendMailAsync(message);
        }
    }
}
