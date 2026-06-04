using HeThongQuanLyThuVien.DTOs.Publishers;

namespace HeThongQuanLyThuVien.Services.Interfaces
{
    /// <summary>
    /// UC20 - Quản lý nhà xuất bản (Thêm / Cập nhật / Xóa)
    /// Xóa: chỉ Admin được phép
    /// </summary>
    public interface IPublisherService
    {
        // GET /publishers
        Task<List<PublisherResponse>> GetListPublishersAsync(CancellationToken ct = default);

        // GET /publishers/:id
        Task<PublisherResponse> GetPublisherByIdAsync(int publisherId, CancellationToken ct = default);

        // POST /publishers
        Task<PublisherResponse> AddPublisherAsync(PublisherRequest request, CancellationToken ct = default);

        // PUT /publishers/:id
        Task UpdatePublisherAsync(int publisherId, PublisherRequest request, CancellationToken ct = default);

        // DELETE /publishers/:id  (chỉ Admin)
        Task DeletePublisherAsync(int publisherId, CancellationToken ct = default);
    }
}