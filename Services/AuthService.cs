using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Auth;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            ApplicationDbContext context, 
            IJwtService jwtService,
            IMailService mailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtService = jwtService;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
            if (emailExists)
                throw new ConflictException("Email da ton tai!");

            var readerRoleId = await _context.Roles
                .AsNoTracking()
                .Where(r => r.RoleName == RoleName.READER)
                .Select(r => (int?)r.RoleId)
                .FirstOrDefaultAsync(ct);

            if (readerRoleId == null)
                throw new NotFoundException("Khong tim thay vai tro nguoi doc!");

            var user = new User
            {
                RoleId = readerRoleId.Value,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                Address = string.Empty,
                Status = UserStatus.Active,
            };

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);

            var libraryCard = new LibraryCard
            {
                UserId = user.UserId,
                LibraryCardCode = GenerateLibraryCardCode(),
                Status = CardStatus.Active,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddYears(3),
            };

            await _context.LibraryCards.AddAsync(libraryCard, ct);
            await _context.SaveChangesAsync(ct);

            return CreateAuthResponse(user.UserId, user.FullName, user.Email, RoleName.READER);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var userExist = await _context.Users
                .AsNoTracking()
                .Where(u => u.Email == request.Email)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.PasswordHash,
                    u.Status,
                    RoleName = u.Role.RoleName
                })
                .FirstOrDefaultAsync(ct);

            if (userExist is null)
                throw new UnauthorizedException("Tai khoan Email khong ton tai!");

            if (userExist.Status == UserStatus.Locked)
                throw new ForbiddenException("Tai khoan da bi khoa!");

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, userExist.PasswordHash);
            if (!isPasswordCorrect)
                throw new UnauthorizedException("Email hoac mat khau khong dung!");

            return CreateAuthResponse(userExist.UserId, userExist.FullName, userExist.Email, userExist.RoleName);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
        {
            // 4. He thong kiem tra email ton tai
            var existEmail = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
            //4a. Email co ton tai khong
            if (!existEmail)
            {
                throw new NotFoundException("Email khong hop le!");
            }

            // khoi tao password moi cap nhat du lieu va gui toi nguoi dung
            var newPassword = Guid.NewGuid().ToString("N")[..6];

            // ExecuteUpdateAsync dich luon ra sql ko can quan EntityTrackingChange nen ko can SaveCangesAsync() commit
            // ham ExecuteUpdateAsync nhan 1 lambda object de cau hinh cac field can cap nhat 
            // ham SetProperty() dung de set cot ben trong truyen lambda lay truong cua bang vd x => x.PasswordHash lay truong PasswordHash
            await _context.Users
                .Where(u => u.Email == request.Email)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(
                        x => x.PasswordHash, 
                        BCrypt.Net.BCrypt.HashPassword(newPassword)));


            // gui password moi toi nguoi dung
            await _mailService.SendEmailAsync(
                request.Email,
                "Reset Password",
                $"Mat khau moi cua ban la: {newPassword}");

        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
        {
            // 3a. Xac nhan mat khau khong khop
            if(request.Password != request.ConfirmPassword)
            {
                throw new ConflictException("Xac nhan mat khau khong khop");
            }

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null)
            {
                throw new UnauthorizedException("nguoi dung ko duoc uy quyen");
            }

            var PasswordOfUser = await _context.Users
                .Where(u => u.UserId == int.Parse(userId))
                .Select(u => u.PasswordHash)
                .FirstOrDefaultAsync(ct);
            
            if(PasswordOfUser == null)
            {
                throw new NotFoundException("Khong tim thay nguoi dung!");
            }
            // 4a. Kiem tra mat khau cu co trung ko
            bool IsPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.OldPassword, PasswordOfUser);

            if (!IsPasswordCorrect)
            {
                throw new ConflictException("Mat khau cu khong dung!");
            }

            var NewPasswordHashed = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _context.Users
                .Where(u => u.UserId == int.Parse(userId))
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(
                        f => f.PasswordHash, 
                        NewPasswordHashed), 
                        ct);

        }
        // Private helpers

        private LoginResponse CreateAuthResponse(int userId, string fullName, string email, RoleName roleName)
        {
            var userForToken = new User
            {
                UserId = userId,
                FullName = fullName,
                Email = email,
                Role = new Role { RoleName = roleName }
            };

            var accessToken = _jwtService.GenerateToken(userForToken);

            return new LoginResponse(
                accessToken,
                new UserInfoResponse
                {
                    UserId = userId,
                    FullName = fullName,
                    Email = email,
                    Role = roleName.ToString(),
                }
            );
        }

        private static string GenerateLibraryCardCode()
            => $"LIB-{Guid.NewGuid().ToString("N").ToUpper()[..8]}";
    }
}