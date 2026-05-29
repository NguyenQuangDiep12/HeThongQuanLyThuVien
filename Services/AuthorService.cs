using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Authors;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;

        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuthorResponse>> GetAuthorsAsync(CancellationToken ct = default)
        {
            return await _context.Authors
                .AsNoTracking()
                .Select(a => new AuthorResponse(a.AuthorId, a.AuthorName, a.Biography ?? string.Empty, a.AuthorUrl ?? string.Empty))
                .ToListAsync(ct);
        }

        public async Task<AuthorResponse> GetAuthorByIdAsync(int id, CancellationToken ct = default)
        {
            var author = await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AuthorId == id, ct);

            if (author is null)
                throw new NotFoundException("Tac gia khong ton tai!");

            return new AuthorResponse(author.AuthorId, author.AuthorName, author.Biography ?? string.Empty, author.AuthorUrl ?? string.Empty);
        }

        public async Task<AuthorResponse> CreateAuthorAsync(AuthorRequest request, CancellationToken ct = default)
        {
            var author = new Author
            {
                AuthorName = request.AuthorName,
                Biography = request.Biography,
                AuthorUrl = request.AuthorUrl
            };

            await _context.Authors.AddAsync(author, ct);
            await _context.SaveChangesAsync(ct);

            return new AuthorResponse(author.AuthorId, author.AuthorName, author.Biography ?? string.Empty, author.AuthorUrl ?? string.Empty);
        }

        public async Task UpdateAuthorAsync(int id, AuthorRequest request, CancellationToken ct = default)
        {
            int rows = await _context.Authors
                .Where(a => a.AuthorId == id)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(a => a.AuthorName, request.AuthorName)
                     .SetProperty(a => a.Biography, request.Biography)
                     .SetProperty(a => a.AuthorUrl, request.AuthorUrl), ct);

            if (rows == 0)
                throw new NotFoundException("Tac gia khong ton tai!");
        }

        public async Task DeleteAuthorAsync(int id, CancellationToken ct = default)
        {
            int rows = await _context.Authors
                .Where(a => a.AuthorId == id)
                .ExecuteDeleteAsync(ct);

            if (rows == 0)
                throw new NotFoundException("Tac gia khong ton tai!");
        }
    }
}