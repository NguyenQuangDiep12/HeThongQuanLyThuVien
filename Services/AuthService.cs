using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Auth;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
            if (emailExists)
                throw new ConflictException("Email da ton tai!"); // request hop le, email dung format, nhung du lieu bi xung dot voi du lieu trong db

            var readerRoleId = await _context.Roles
                .AsNoTracking()
                .Where(r => r.RoleName == RoleName.READER)
                .Select(r => (int?)r.RoleId)
                .FirstOrDefaultAsync(ct);

            if (readerRoleId is null)
                throw new NotFoundException("Khong tim thay vai tro nguoi doc!");

            var user = new User
            {
                RoleId = readerRoleId.Value,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                Address = string.Empty,
                LibraryCardCode = GenerateLibraryCardCode(),
                CardStatus = CardStatus.PENDING,
            };

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);

            return CreateAuthResponse(user.UserId, user.FullName, user.Email, RoleName.READER);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            // 1. Kiem tra xem Email nguoi dung da dang ky chua
            var userExist = await _context.Users
                .AsNoTracking()
                .Where(u => u.Email == request.Email)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.PasswordHash,
                    u.Role.RoleName
                })
                .FirstOrDefaultAsync(ct);

            if (userExist == null)
            {
                throw new UnauthorizedException("Tai khoan Email khong ton tai!");
            }

            // 2. nguoi dung ton tai kiem tra mat khau 
            bool IsPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, userExist.PasswordHash);
            if (!IsPasswordCorrect)
            {
                throw new UnauthorizedException("Email hoac mat khau khong dung!");
            }

            return CreateAuthResponse(userExist.UserId, userExist.FullName, userExist.Email, userExist.RoleName);
        }

        // Private helpers
        private LoginResponse CreateAuthResponse(int userId, string fullName, string email, RoleName roleName)
        {
            var user = new User
            {
                UserId = userId,
                FullName = fullName,
                Email = email,
                Role = new Role { RoleName = roleName }
            };

            var accessToken = _jwtService.GenerateToken(user);

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
            => $"Library-{Guid.NewGuid().ToString("N").ToUpper()[..8]}";
    }
}
