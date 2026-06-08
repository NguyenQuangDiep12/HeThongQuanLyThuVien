using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Authors;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;
        public AuthorService(
            ApplicationDbContext context,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }
        // | GET | /authors/:id | Chi tiết tác giả | Public |
        public async Task<AuthorResponse> GetAuthorByIdAsync(int id, CancellationToken ct = default)
        {
            var author = await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AuthorId == id, ct);

            if (author is null)
                throw new NotFoundException("Tac gia khong ton tai!");

            return new AuthorResponse(
                author.AuthorId, 
                author.AuthorName, 
                author.Biography ?? string.Empty, 
                author.AuthorUrl ?? string.Empty
            );
        }
        // | PUT | /authors/:id | Cập nhật tác giả | Staff/Admin |
        public async Task UpdateAuthorAsync(int id, AuthorRequest request, CancellationToken ct = default)
        {
            var query = _context.Authors;

            bool exist = await query.AsNoTracking().AnyAsync(a => a.AuthorId == id, ct);
            if (!exist) throw new NotFoundException("Khong tim thay tac gia!");

            int rows = await query
                .Where(a => a.AuthorId == id)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(a => a.AuthorName, request.AuthorName)
                     .SetProperty(a => a.Biography, request.Biography)
                     .SetProperty(a => a.AuthorUrl, request.AuthorUrl), ct);

            if (rows == 0)
                throw new NotFoundException("Tac gia khong ton tai!");
        }
        // | DELETE | /authors/:id | Xóa tác giả | Admin |
        public async Task DeleteAuthorAsync(int id, CancellationToken ct = default)
        {

            bool exist = await _context.Authors.AnyAsync(a => a.AuthorId == id);

            if (!exist)
            {
                throw new NotFoundException("Tac gia khong ton tai!");
            }
            
            int rows = await _context.Authors.Where(a => a.AuthorId == id).ExecuteDeleteAsync(ct);

            if (rows == 0) throw new NotFoundException("Tac gia khong ton tai!");
        }
        // | GET | /authors | Danh sách tác giả | Public |
        public async Task<List<AuthorResponse>> GetListAuthorsAsync(CancellationToken ct = default)
        {
            return await _context.Authors
                .AsNoTracking()
                .Select(a => new AuthorResponse(
                    a.AuthorId, 
                    a.AuthorName, 
                    a.Biography ?? string.Empty, 
                    a.AuthorUrl ?? string.Empty
                ))
                .ToListAsync(ct);
        }
        // | POST | /authors | Thêm tác giả | Staff/Admin |
        public async Task<AuthorResponse> AddAuthorAsync(AuthorRequest request, CancellationToken ct = default)
        {
            bool exist = await _context.Authors
                .AnyAsync(a => a.AuthorName == request.AuthorName, ct);
            if (exist) throw new ConflictException("Tac gia da ton tai!");

            var author = new Author
            {
                AuthorName = request.AuthorName,
                Biography = request.Biography,
                AuthorUrl = request.AuthorUrl
            };

            await _context.Authors.AddAsync(author, ct);
            await _context.SaveChangesAsync(ct);

            return new AuthorResponse(
                author.AuthorId, 
                author.AuthorName, 
                author.Biography ?? string.Empty, 
                author.AuthorUrl ?? string.Empty
            );
        }
    }
}