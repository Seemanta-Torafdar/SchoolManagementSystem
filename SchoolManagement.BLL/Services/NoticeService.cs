using SchoolManagement.BLL.DTOs.Notice;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.Services;

public class NoticeService : INoticeService
{
    private readonly INoticeRepository _noticeRepo;
    private readonly IAccountService _accountService;

    public NoticeService(INoticeRepository noticeRepo, IAccountService accountService)
    {
        _noticeRepo = noticeRepo;
        _accountService = accountService;
    }

    public async Task<NoticeDto?> GetByIdAsync(int id, bool incrementView = false)
    {
        var n = await _noticeRepo.GetByIdAsync(id);
        if (n == null) return null;

        if (incrementView)
        {
            await _noticeRepo.IncrementViewCountAsync(id);
        }

        return MapToDto(n);
    }

    public async Task<IReadOnlyList<NoticeDto>> GetPublicNoticesAsync(int count = 10)
    {
        var items = await _noticeRepo.GetPublicNoticesAsync(count);
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<NoticeDto>> GetTeacherNoticesAsync(int count = 10)
    {
        var items = await _noticeRepo.GetTeacherNoticesAsync(count);
        return items.Select(MapToDto).ToList();
    }

    public async Task<PaginatedList<NoticeDto>> SearchNoticesAsync(string? searchTerm, NoticeType? category, NoticeStatus? status, int page, int pageSize)
    {
        var items = await _noticeRepo.SearchNoticesAsync(searchTerm, category, status, page, pageSize);
        var total = await _noticeRepo.GetTotalCountAsync(searchTerm, category, status);
        var dtos = items.Select(MapToDto).ToList();
        return new PaginatedList<NoticeDto>(dtos, total, page, pageSize);
    }

    public async Task<ResultResponse<NoticeDto>> CreateAsync(CreateNoticeDto dto, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
            return ResultResponse<NoticeDto>.Failure("Notice title and content are required.");

        var notice = new Notice
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            Category = dto.Category,
            Status = dto.Status,
            PublishDate = dto.PublishDate,
            ExpiryDate = dto.ExpiryDate,
            AttachmentUrl = dto.AttachmentUrl,
            IsPublic = dto.IsPublic,
            IsTeacherOnly = dto.IsTeacherOnly,
            ViewCount = 0
        };

        var created = await _noticeRepo.AddAsync(notice);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Notice", created.Id.ToString(), $"Published notice '{created.Title}'");

        return ResultResponse<NoticeDto>.Success(MapToDto(created), "Notice created successfully.");
    }

    public async Task<ResultResponse> UpdateAsync(UpdateNoticeDto dto, string? currentUserId = null)
    {
        var notice = await _noticeRepo.GetByIdAsync(dto.Id);
        if (notice == null)
            return ResultResponse.Failure("Notice not found.");

        notice.Title = dto.Title.Trim();
        notice.Content = dto.Content.Trim();
        notice.Category = dto.Category;
        notice.Status = dto.Status;
        notice.PublishDate = dto.PublishDate;
        notice.ExpiryDate = dto.ExpiryDate;
        if (!string.IsNullOrWhiteSpace(dto.AttachmentUrl))
            notice.AttachmentUrl = dto.AttachmentUrl;
        notice.IsPublic = dto.IsPublic;
        notice.IsTeacherOnly = dto.IsTeacherOnly;

        await _noticeRepo.UpdateAsync(notice);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "Notice", notice.Id.ToString(), $"Updated notice '{notice.Title}'");

        return ResultResponse.Success("Notice updated successfully.");
    }

    public async Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null)
    {
        var notice = await _noticeRepo.GetByIdAsync(id);
        if (notice == null)
            return ResultResponse.Failure("Notice not found.");

        await _noticeRepo.DeleteAsync(notice);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Notice", id.ToString(), $"Deleted notice '{notice.Title}'");

        return ResultResponse.Success("Notice deleted successfully.");
    }

    public async Task<int> GetActiveNoticesCountAsync()
    {
        return await _noticeRepo.CountAsync(n => n.Status == NoticeStatus.Published);
    }

    private static NoticeDto MapToDto(Notice n)
    {
        return new NoticeDto
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            Category = n.Category,
            Status = n.Status,
            PublishDate = n.PublishDate,
            ExpiryDate = n.ExpiryDate,
            AttachmentUrl = n.AttachmentUrl,
            IsPublic = n.IsPublic,
            IsTeacherOnly = n.IsTeacherOnly,
            ViewCount = n.ViewCount
        };
    }
}

