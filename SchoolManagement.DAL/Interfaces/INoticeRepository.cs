using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.DAL.Interfaces;

public interface INoticeRepository : IGenericRepository<Notice>
{
    Task<IReadOnlyList<Notice>> GetPublicNoticesAsync(int count = 10);
    Task<IReadOnlyList<Notice>> GetTeacherNoticesAsync(int count = 10);
    Task<IReadOnlyList<Notice>> SearchNoticesAsync(string? searchTerm, NoticeType? category, NoticeStatus? status, int page, int pageSize);
    Task<int> GetTotalCountAsync(string? searchTerm, NoticeType? category, NoticeStatus? status);
    Task IncrementViewCountAsync(int noticeId);
}

