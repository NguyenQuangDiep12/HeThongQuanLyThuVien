using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.DTOs.Categories;
using HeThongQuanLyThuVien.Exceptions;
using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace HeThongQuanLyThuVien.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public CategoryService(
            ApplicationDbContext context,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<CategoryResponse> GetCategoryByIdAsync(int id, CancellationToken ct = default)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id, ct);

            if (category is null)
                throw new NotFoundException("Danh muc khong ton tai!");

            return new CategoryResponse
            (
                category.CategoryId,
                category.CategoryName,
                category.Description ?? string.Empty
            );
        }


        public async Task UpdateCategoryAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default)
        {
            int rows = await _context.Categories
                .Where(c => c.CategoryId == id)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(c => c.CategoryName, request.CategoryName)
                     .SetProperty(c => c.Description, request.Description), ct);

            if (rows == 0)
                throw new NotFoundException("Danh muc khong ton tai!");
        }

        public async Task DeleteCategoryAsync(int id, CancellationToken ct = default)
        {
            var roleUser = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleUser != "ADMIN")
            {
                throw new UnauthorizedException("Nguoi dung khong co quyen thuc hien chuc nang nay!");
            }

            // Kiem tra rang buoc — xoa danh muc con sach thi cascade xoa BookCategory
            int rows = await _context.Categories
                .Where(c => c.CategoryId == id)
                .ExecuteDeleteAsync(ct);

            if (rows == 0)
                throw new NotFoundException("Danh muc khong ton tai!");
        }

        public async Task<List<CategoryResponse>> GetListCategoriesAsync(CancellationToken ct = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryResponse
                (
                    c.CategoryId,
                    c.CategoryName,
                    c.Description ?? string.Empty
                ))
                .ToListAsync(ct);
        }

        public async Task<CategoryResponse> AddCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
        {
            var category = new Category
            {
                CategoryName = request.CategoryName,
                Description = request.Description
            };

            await _context.Categories.AddAsync(category, ct);
            await _context.SaveChangesAsync(ct);

            return new CategoryResponse
            (
                category.CategoryId,
                category.CategoryName,
                category.Description ?? string.Empty
            );
        }
    }
}