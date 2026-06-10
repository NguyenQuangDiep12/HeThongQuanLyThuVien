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
        // | GET | /staffs | Danh sách nhân viên | Admin |
        public async Task<PaginationResponse<UserListInfoResponse>> GetStaffsAsync(
            GetUserRequest request, CancellationToken ct = default)
        {
            var query = _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == RoleName.STAFF);

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                query = query.Where(u =>
                    u.FullName.Contains(request.FullName));
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                query = query.Where(u =>
                    u.Email.Contains(request.Email));
            }

            if (request.Status != null)
            {
                query = query.Where(u =>
                    u.Status == request.Status);
            }

            if (request.RoleName != null)
            {
                query = query.Where(u =>
                    u.Role.RoleName.ToString() == request.RoleName);
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
                    CardStatus = u.LibraryCard != null ? u.LibraryCard.Status.ToString() : string.Empty
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
        public async Task<UserProfileResponse> GetUserByIdAsync(int userId, int currentUserId, string currentRole,CancellationToken ct = default)
        {


            bool isAdmin = currentRole == "ADMIN";
            bool isStaff = currentRole == "STAFF";
            bool isOwner = currentUserId == userId;

            // Kiem tra quyen xem co ban (Chan truy van neu Reader di xem nguoi khac)
            // READER: chi duoc xem chinh minh

            // TH1: Reader xem chinh minh => IsAdmin = !false => true, IsStaff = !false => true, IsOwner = !true => false , => false
            
            // TH2: Reader xem thong tin nguoi khac IsAdmin = !false => true, IsStaff = !false => true, IsOwner = !false => true , => true
            if (!isAdmin && !isStaff && !isOwner)
                throw new ForbiddenException("Ban khong co quyen xem thong tin nguoi dung nay!");

            // 2. TRUY VẤN 1 LẦN DUY NHẤT: Lay thong tin user can xem
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .Select(u => new UserProfileResponse
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Address,
                    AvatarUrl = u.AvatarUrl,
                    Role = u.Role.RoleName.ToString(), // Dung de check quyen ben duoi
                    Status = u.Status.ToString(),
                    CardStatus = u.LibraryCard != null ? u.LibraryCard.Status.ToString() : string.Empty,
                    LibraryCardCode = u.LibraryCard != null ? u.LibraryCard.LibraryCardCode : "",
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (user == null)
                throw new NotFoundException("Nguoi dung khong ton tai!");

            // 3. Kiem tra quyen xem chi tiet (Phan cap)
            // STAFF chi duoc xem chinh minh (isOwner) hoac xem READER. 
            // Khong duoc xem ADMIN va các STAFF khac.
            if (isStaff && !isOwner && user.Role != "READER")
            {
                throw new ForbiddenException("Ban khong co quyen xem thong tin cua nhan vien hoac quan tri vien khac!");
            }
            /*
             * TH1: Staff xem chinh minh
             * isStaff = true
             * isOwner = true
             * !isOwner = false
             * => Dieu kien = false => Cho phep
             *
             * TH2: Staff xem Reader
             * isStaff = true
             * isOwner = false
             * !isOwner = true
             * user.Role = READER => user.Role != "READER" = false
             * => Dieu kien = false => Cho phep
             *
             * TH3: Staff xem Admin
             * isStaff = true
             * isOwner = false
             * !isOwner = true
             * user.Role != "READER" = true
             * => Dieu kien = true => Forbidden
             *
             * TH4: Staff xem Staff khac
             * isStaff = true
             * isOwner = false
             * !isOwner = true
             * user.Role != "READER" = true
             * => Dieu kien = true => Forbidden
             */
            return user;
        }
        // GET /users - STAFF/ADMIN Xem danh sach nguoi dung
        public async Task<PaginationResponse<UserListInfoResponse>> GetUsersAsync(GetUserRequest request, string currentUserRole, CancellationToken ct = default)
        {

            IQueryable<User> query = _context.Users.AsNoTracking();

            // STAFF chỉ xem được READER, ADMIN xem được tất cả
            if (currentUserRole == RoleName.STAFF.ToString())
            {
                query = query.Where(u => u.Role.RoleName == RoleName.READER);
            }
            else if (currentUserRole != RoleName.ADMIN.ToString())
            {
                throw new ForbiddenException(
                    "Ban khong co quyen xem danh sach nguoi dung!");
            }
            // ADMIN không filter role => xem toàn bộ

            // Các filter động
            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                query = query.Where(u => u.FullName.Contains(request.FullName.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                query = query.Where(u => u.Email.Contains(request.Email.Trim()));
            }
            if (request.Status != null)
            {
                query = query.Where(u => u.Status == request.Status);
            }

            int total = await query.CountAsync(ct);
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
                    CardStatus = u.LibraryCard != null ? u.LibraryCard.Status.ToString() : string.Empty,
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
            var card = await _context.LibraryCards.FirstOrDefaultAsync(l => l.UserId == userId, ct);
            if(card == null) throw new NotFoundException("The thu vien khong ton tai!");

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
        public async Task UpdateUserAsync(int userId, UpdateUserRequest request, int currentUserId, string currentRole, CancellationToken ct = default)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId, ct);
            if (user == null)
                throw new NotFoundException("Nguoi dung khong ton tai!");
            
            // STAFF khong duoc sua chinh minh
            if(currentRole == "STAFF" && currentUserId == userId)
            {
                throw new ForbiddenException("STAFF khong duoc tu sua thong tin cua endpoint nay!");
            }

            // STAFF chi duoc sua reader
            if(currentRole == "STAFF" && user.Role.RoleName != RoleName.READER)
            {
                throw new ForbiddenException("STAFF chi duoc sua reader!");
            }
            user.FullName = request.FullName;
            user.Phone = request.Phone;
            user.Address = request.Address;
            user.UpdatedAt = DateTime.UtcNow;

            // chi ADMIN moi co quyen thay doi RoleId cua nguoi khac
            if (currentRole == "ADMIN")
            {
                bool roleExists = await _context.Roles.AnyAsync(r => r.RoleId == request.RoleId, ct);
                if (!roleExists)
                {
                    throw new NotFoundException("Vai tro khong ton tai!");
                }
                user.RoleId = request.RoleId;
            }

            await _context.SaveChangesAsync(ct);
        }
        // PATCH /users/:id/status  Khóa / mở tài khoản  Admin
        public async Task UpdateUserStatusAsync(int userId, UpdateUserStatusRequest request, CancellationToken ct = default)
        {

            int affectedRows = await _context.Users
                .Where(u => u.UserId == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.Status, request.Status)
                    .SetProperty(u => u.UpdatedAt, DateTime.UtcNow)
                , ct);

            if (affectedRows == 0)
                throw new NotFoundException("Nguoi dung khong ton tai!");
        }
    }
}
