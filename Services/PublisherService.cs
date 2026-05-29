using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Publishers;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLyThuVien.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly ApplicationDbContext _context;

        public PublisherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PublisherResponse>> GetPublishersAsync(CancellationToken ct = default)
        {
            return await _context.Publisher
                .AsNoTracking()
                .Select(p => new PublisherResponse
                {
                    PublisherId = p.PublisherId,
                    PublisherName = p.PublisherName,
                    LogoUrl = p.LogoUrl
                })
                .ToListAsync(ct);
        }

        public async Task<PublisherResponse> GetPublisherByIdAsync(int id, CancellationToken ct = default)
        {
            var publisher = await _context.Publisher
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PublisherId == id, ct);

            if (publisher is null)
                throw new NotFoundException("Nha xuat ban khong ton tai!");

            return new PublisherResponse
            {
                PublisherId = publisher.PublisherId,
                PublisherName = publisher.PublisherName,
                LogoUrl = publisher.LogoUrl
            };
        }

        public async Task<PublisherResponse> CreatePublisherAsync(CreatePublisherRequest request, CancellationToken ct = default)
        {
            var publisher = new Publisher
            {
                PublisherName = request.PublisherName,
                LogoUrl = request.LogoUrl
            };

            await _context.Publisher.AddAsync(publisher, ct);
            await _context.SaveChangesAsync(ct);

            return new PublisherResponse
            {
                PublisherId = publisher.PublisherId,
                PublisherName = publisher.PublisherName,
                LogoUrl = publisher.LogoUrl
            };
        }

        public async Task UpdatePublisherAsync(int id, UpdatePublisherRequest request, CancellationToken ct = default)
        {
            int rows = await _context.Publisher
                .Where(p => p.PublisherId == id)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.PublisherName, request.PublisherName)
                     .SetProperty(p => p.LogoUrl, request.LogoUrl), ct);

            if (rows == 0)
                throw new NotFoundException("Nha xuat ban khong ton tai!");
        }

        public async Task DeletePublisherAsync(int id, CancellationToken ct = default)
        {
            // Kiem tra con sach lien ket khong
            bool hasBooks = await _context.Books.AnyAsync(b => b.PublisherId == id, ct);
            if (hasBooks)
                throw new BadRequestException("Khong the xoa nha xuat ban dang co sach lien ket!");

            int rows = await _context.Publisher
                .Where(p => p.PublisherId == id)
                .ExecuteDeleteAsync(ct);

            if (rows == 0)
                throw new NotFoundException("Nha xuat ban khong ton tai!");
        }
    }
}