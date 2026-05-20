using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Auth;
using HeThongQuanLyThuVien.DTOs.Shared;
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
                throw new InvalidOperationException("Email da ton tai!");

            var readerRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == RoleName.READER, ct);

            var user = new User
            {
                RoleId = readerRole!.RoleId,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                Address = string.Empty,
                LibraryCardCode = GenerateLibraryCardCode(),
                CardStatus = CardStatus.PENDING,
                Role = readerRole,
            };

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);

            return BuildLoginResponse(user);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Email hoac mat khau khong dung!");

            return BuildLoginResponse(user);
        }

        // Private helpers
        private LoginResponse BuildLoginResponse(User user)
        {
            var accessToken = _jwtService.GenerateToken(user);

            return new LoginResponse
            {
                AccessToken = accessToken,
                UserInfo = new UserInfoResponse
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role.RoleName.ToString(),
                }
            };
        }

        private static string GenerateLibraryCardCode()
            => $"Library-{Guid.NewGuid().ToString("N").ToUpper()[..8]}";
    }
}