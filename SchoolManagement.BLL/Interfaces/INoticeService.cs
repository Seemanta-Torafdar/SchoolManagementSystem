using SchoolManagement.BLL.DTOs.Notice;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.Interfaces;

public interface INoticeService
{
    Task<NoticeDto?> GetByIdAsync(int id, bool incrementView = false);
    Task<IReadOnlyList<NoticeDto>> GetPublicNoticesAsync(int count = 10);
    Task<IReadOnlyList<NoticeDto>> GetTeacherNoticesAsync(int count = 10);
    Task<PaginatedList<NoticeDto>> SearchNoticesAsync(string? searchTerm, NoticeType? category, NoticeStatus? status, int page, int pageSize);
    Task<ResultResponse<NoticeDto>> CreateAsync(CreateNoticeDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateAsync(UpdateNoticeDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null);
    Task<int> GetActiveNoticesCountAsync();
}

