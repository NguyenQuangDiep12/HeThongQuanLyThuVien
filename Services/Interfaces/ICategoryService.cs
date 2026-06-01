using HeThongQuanLyThuVien.DTOs.Categories;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC18 - Quản lý danh mục (Thêm / Cập nhật / Xóa)
    /// Xóa: chỉ Admin được phép
    /// </summary>
    public interface ICategoryService
    {
        // GET /categories
        Task<List<CategoryResponse>> GetListCategoriesAsync(CancellationToken ct = default);

        // GET /categories/:id
        Task<CategoryResponse> GetCategoryByIdAsync(int categoryId, CancellationToken ct = default);

        // POST /categories
        Task<CategoryResponse> AddCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default);

        // PUT /categories/:id
        Task UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request, CancellationToken ct = default);

        // DELETE /categories/:id  (chỉ Admin)
        Task DeleteCategoryAsync(int categoryId, CancellationToken ct = default); 
    }
}