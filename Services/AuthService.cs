using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Auth;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

            return CreateLoginResponse(user);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var sw = Stopwatch.StartNew();
            // 1. Kiem tra xem Email nguoi dung da dang ky chua
            var userExist = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            Console.WriteLine($"Query DB: {sw.ElapsedMilliseconds} ms");

            if (userExist == null)
            {
                throw new UnauthorizedException("Tai khoan Email khong ton tai!");
            }

            sw.Restart();

            // 2. nguoi dung ton tai kiem tra mat khau 
            bool IsPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, userExist.PasswordHash);
            if (!IsPasswordCorrect)
            {
                throw new UnauthorizedException("Email hoac mat khau khong dung!");
            }
            Console.WriteLine($"BCrypt Verify: {sw.ElapsedMilliseconds} ms");

            return CreateLoginResponse(userExist);
        }

        // Private helpers
        private LoginResponse CreateLoginResponse(User user)
        {
            var accessToken = _jwtService.GenerateToken(user);

            return new LoginResponse(
                accessToken,
                new UserInfoResponse
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role.RoleName.ToString(),
                }
            );
        }

        private static string GenerateLibraryCardCode()
            => $"Library-{Guid.NewGuid().ToString("N").ToUpper()[..8]}";
    }
}