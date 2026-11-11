using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IGalleryService
{
    Task<IReadOnlyList<GalleryDto>> GetActiveGalleryAsync(string? category = null);
    Task<IReadOnlyList<GalleryDto>> GetAllGalleryAsync();
    Task<GalleryDto?> GetByIdAsync(int id);
    Task<ResultResponse<GalleryDto>> CreateAsync(GalleryDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null);
}

