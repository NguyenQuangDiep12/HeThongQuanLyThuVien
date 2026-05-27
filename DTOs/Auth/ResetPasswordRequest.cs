namespace HeThongQuanLyThuVien.DTOs.Auth
{
    public record ResetPasswordRequest(string Password, string ConfirmPassword, string OldPassword);
}
