using Microsoft.EntityFrameworkCore;
using SchoolManagement.DAL.Data;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.DAL.Repositories;

public class NoticeRepository : GenericRepository<Notice>, INoticeRepository
{
    public NoticeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Notice>> GetPublicNoticesAsync(int count = 10)
    {
        return await _dbSet
            .Where(n => n.IsPublic && n.Status == NoticeStatus.Published && (!n.ExpiryDate.HasValue || n.ExpiryDate.Value >= DateTime.UtcNow))
            .OrderByDescending(n => n.PublishDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Notice>> GetTeacherNoticesAsync(int count = 10)
    {
        return await _dbSet
            .Where(n => (n.IsTeacherOnly || n.IsPublic) && n.Status == NoticeStatus.Published && (!n.ExpiryDate.HasValue || n.ExpiryDate.Value >= DateTime.UtcNow))
            .OrderByDescending(n => n.PublishDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Notice>> SearchNoticesAsync(string? searchTerm, NoticeType? category, NoticeStatus? status, int page, int pageSize)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(n => n.Title.ToLower().Contains(term) || n.Content.ToLower().Contains(term));
        }

        if (category.HasValue)
            query = query.Where(n => n.Category == category.Value);

        if (status.HasValue)
            query = query.Where(n => n.Status == status.Value);

        return await query
            .OrderByDescending(n => n.PublishDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm, NoticeType? category, NoticeStatus? status)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(n => n.Title.ToLower().Contains(term) || n.Content.ToLower().Contains(term));
        }

        if (category.HasValue)
            query = query.Where(n => n.Category == category.Value);

        if (status.HasValue)
            query = query.Where(n => n.Status == status.Value);

        return await query.CountAsync();
    }

    public async Task IncrementViewCountAsync(int noticeId)
    {
        var notice = await _dbSet.FindAsync(noticeId);
        if (notice != null)
        {
            notice.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}

