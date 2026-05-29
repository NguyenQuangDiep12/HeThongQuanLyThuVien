using Azure.Core;
using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Shared;
using HeThongQuanLyThuVien.DTOs.Users;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        // POST /staff - admin them nhan vien
        public async Task CreateStaffAsync(CreateStaffRequest request, CancellationToken ct = default)
        {
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email);

            if (emailExists)
            {
                throw new ConflictException("Email da ton tai!");
            }

            int? staffRoleId = await _context.Roles
                .Where(r => r.RoleName == RoleName.STAFF)
                .Select(r => (int?)r.RoleId) // ep kieu ve int nullable vi firstordefault co the tra ve null neu nhu ko tim thay kq 
                .FirstOrDefaultAsync(ct);
            // neu ko ep kieu (int?)r.RoleId thi se tra ve 0 thay vi null vi vay khong the xac dinh duoc Role cua nguoi dung

            
            if(staffRoleId == null)
            {
                throw new NotFoundException("Khong tim thay vai tro cua nhan vien!");
            }

            var staff = new User
            {
                RoleId = staffRoleId.Value,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                Address = request.Address,
                CreatedAt = DateTime.UtcNow
            };

            // nhan vien khong can the thu vien
            await _context.Users.AddAsync(staff);
            await _context.SaveChangesAsync(ct);
        }
        // GET /users - Staff/Admin xem danh sach nguoi dung 
        public async Task<PaginationResponse<UserListInfoResponse>> GetStaffsAsync(
            GetUserRequest request, CancellationToken ct = default)
        {
            var query = _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == RoleName.STAFF);

            if(!string.IsNullOrWhiteSpace(request.FullName) || request.Status != null || !string.IsNullOrWhiteSpace(request.Email) || request.Role != null)
            {
                var FullNameKeyword = request.FullName?.Trim();
                var EmailKeyword = request.Email?.Trim();

                query = query.Where(u =>
                    u.FullName == FullNameKeyword ||
                    u.Email == EmailKeyword ||
                    u.Status == request.Status ||
                    u.RoleId == request.Role
                );
            }

            int total = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.UserId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserListInfoResponse
                {
                    Id = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role.RoleName.ToString(),
                    Status = u.Status.ToString(),
                    CardStatus = u.LibraryCard.Status.ToString(),
                }).ToListAsync(ct);

            return new PaginationResponse<UserListInfoResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }
        // GET /users/:id - xem chi tiet nguoi dung
        public async Task<UserProfileResponse> GetUserByIdAsync(int userId, CancellationToken ct = default)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.LibraryCard)
                .Where(u => u.UserId == userId)
                .Select(u => new UserProfileResponse
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Address,
                    AvatarUrl = u.AvatarUrl,
                    Role = u.Role.RoleName.ToString(),
                    Status = u.Status.ToString(),
                    CardStatus = u.LibraryCard.Status.ToString(),
                    LibraryCardCode = u.LibraryCard != null ? u.LibraryCard.LibraryCardCode : GenerateLibraryCardCodeAsync(_context).ToString()!,
                    CreatedAt = DateTime.UtcNow,
                }).FirstOrDefaultAsync(ct);

            if(user == null)
            {
                throw new NotFoundException("Nguoi dung khong ton tai!");
            }
            return user;
        }
        // 
        public async Task<PaginationResponse<UserListInfoResponse>> GetUsersAsync(GetUserRequest request, CancellationToken ct = default)
        {
            var query = _context.Users
             .AsNoTracking()
             .Include(u => u.Role)
             .Where(u => u.Role.RoleName == RoleName.READER);

            if (!string.IsNullOrWhiteSpace(request.FullName) || request.Status != null || !string.IsNullOrWhiteSpace(request.Email) || request.Role != null)
            {
                var FullNameKeyword = request.FullName?.Trim();
                var EmailKeyword = request.Email?.Trim();

                query = query.Where(u =>
                    u.FullName == FullNameKeyword ||
                    u.Email == EmailKeyword ||
                    u.Status == request.Status ||
                    u.RoleId == request.Role
                );
            }

            int total = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.UserId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserListInfoResponse
                {
                    Id = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role.RoleName.ToString(),
                    Status = u.Status.ToString(),
                    CardStatus = u.LibraryCard.Status.ToString(),
                }).ToListAsync(ct);

            return new PaginationResponse<UserListInfoResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = total
            };
        }
        // PATCH /user/:id/card-status - Admin mo/ khoa the thu vien
        public async Task UpdateLibraryCardStatusAsync(int userId, UpdateLibraryCardStatusRequest request, CancellationToken ct = default)
        {
            // kiem tra the thu vien nguoi dung ton tai hay khong?
            var card = await _context.LibraryCards
                .FirstOrDefaultAsync(l => l.UserId == userId);
            if (card == null)
            {
                throw new NotFoundException("The thu vien khong ton tai!");
            }

            card.Status = request.Status;
            await _context.SaveChangesAsync(ct);

        }
        // PUT /users/me/profile - reader tu cap nhat ho so ca nhan
        public async Task UpdateMyProfileAsync(int currentUserId, UpdateProfileRequest request, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(new object[] { currentUserId });
            if (user == null)
            {
                throw new NotFoundException("Nguoi dung khong ton tai!");
            }

            user.FullName = request.FullName;
            user.Phone = request.Phone;
            user.Address = request.Address;
            user.AvatarUrl = request.AvatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }
        // PUT /staff/:id - admin cap nhat thong tin nhan vien
        public async Task UpdateStaffAsync(int staffId, UpdateStaffRequest request, CancellationToken ct = default)
        {
            var staff = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == staffId);
            if(staff == null)
            {
                throw new NotFoundException("Nhan vien khong ton tai!");
            }

            if(staff.Role.RoleName != RoleName.STAFF)
            {
                throw new ForbiddenException("Nguoi dung nay khong phai nhan vien!");
            }

            staff.FullName = request.FullName;
            staff.Phone = request.Phone;
            staff.Address = request.Address;
            staff.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }
        // PUT /users/:id - STAFF, ADMIN cap nhat thong tin nguoi dung
        public async Task UpdateUserAsync(int userId, UpdateUserRequest request, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId });

            if(user == null)
            {
                throw new NotFoundException("Nguoi dung khong ton tai !");
            }

            user.FullName = request.FullName;
            user.Phone = request.Phone;
            user.Address = request.Address;
            user.RoleId = request.RoleId;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }
        // PATCH /users/:id/status - admin mo/ khoa tai khoan 
        public async Task UpdateUserStatusAsync(int userId, UpdateUserStatusRequest request, CancellationToken ct = default)
        {
            int affectedRows = await _context.Users
                .ExecuteUpdateAsync(setters => 
                    setters
                    .SetProperty(u => u.Status, request.Status)
                    .SetProperty(u => u.UpdatedAt, request.UpdatedAt)
                    ,ct);

            if(affectedRows == 0)
            {
                throw new NotFoundException("Nguoi dung khong ton tai!");
            }
        }

        // private function
        private static async Task<string> GenerateLibraryCardCodeAsync(ApplicationDbContext _context)
        {
            string cardcode;

            do
            {
                // dat theo mau LIB - Year/Month/Day - Random 4 so
                cardcode = $"LIB{DateTime.UtcNow:yyyy:MM:dd}{Random.Shared.Next(1000, 9999)}";
            } while (await _context.LibraryCards.AnyAsync(l => l.LibraryCardCode == cardcode)); // kiem tra xem da co ma the thu vien nao generate trung chua neu co thi generate lai

            return cardcode;
        }
    }
}
