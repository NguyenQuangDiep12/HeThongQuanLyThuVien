using HeThongQuanLyThuVien.DTOs.Authors;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC19 - Quản lý tác giả (Thêm / Cập nhật / Xóa)
    /// Xóa: chỉ Admin được phép
    /// </summary>
    public interface IAuthorService
    {
        // GET /authors
        Task<List<AuthorResponse>> GetListAuthorsAsync(CancellationToken ct = default);

        // GET /authors/:id
        Task<AuthorResponse> GetAuthorByIdAsync(int authorId, CancellationToken ct = default);

        // POST /authors
        Task<AuthorResponse> AddAuthorAsync(AuthorRequest request, CancellationToken ct = default);

        // PUT /authors/:id
        Task UpdateAuthorAsync(int authorId, AuthorRequest request, CancellationToken ct = default);

        // DELETE /authors/:id  (chỉ Admin)
        Task DeleteAuthorAsync(int authorId, CancellationToken ct = default); 
    }
}