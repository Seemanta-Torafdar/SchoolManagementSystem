using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.BLL.DTOs.Event;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.BLL.Services;

public class EventService : IEventService
{
    private readonly ApplicationDbContext _context;
    private readonly IAccountService _accountService;

    public EventService(ApplicationDbContext context, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<IReadOnlyList<EventDto>> GetUpcomingEventsAsync(int count = 6)
    {
        var now = DateTime.UtcNow.Date;
        var events = await _context.Events
            .Where(e => e.IsPublic && e.EventDate >= now)
            .OrderBy(e => e.EventDate)
            .Take(count)
            .ToListAsync();

        return events.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<EventDto>> GetAllEventsAsync()
    {
        var events = await _context.Events
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();

        return events.Select(MapToDto).ToList();
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        var e = await _context.Events.FindAsync(id);
        return e == null ? null : MapToDto(e);
    }

    public async Task<ResultResponse<EventDto>> CreateAsync(CreateEventDto dto, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return ResultResponse<EventDto>.Failure("Event title is required.");

        var ev = new Event
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            EventDate = dto.EventDate,
            Location = dto.Location.Trim(),
            BannerImageUrl = dto.BannerImageUrl,
            IsPublic = dto.IsPublic
        };

        await _context.Events.AddAsync(ev);
        await _context.SaveChangesAsync();
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Event", ev.Id.ToString(), $"Created event '{ev.Title}'");

        return ResultResponse<EventDto>.Success(MapToDto(ev), "Event created successfully.");
    }

    public async Task<ResultResponse> UpdateAsync(int id, CreateEventDto dto, string? currentUserId = null)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null)
            return ResultResponse.Failure("Event not found.");

        ev.Title = dto.Title.Trim();
        ev.Description = dto.Description.Trim();
        ev.EventDate = dto.EventDate;
        ev.Location = dto.Location.Trim();
        if (!string.IsNullOrWhiteSpace(dto.BannerImageUrl))
            ev.BannerImageUrl = dto.BannerImageUrl;
        ev.IsPublic = dto.IsPublic;

        await _context.SaveChangesAsync();
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "Event", id.ToString(), $"Updated event '{ev.Title}'");

        return ResultResponse.Success("Event updated successfully.");
    }

    public async Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null)
            return ResultResponse.Failure("Event not found.");

        ev.IsDeleted = true;
        await _context.SaveChangesAsync();
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Event", id.ToString(), $"Deleted event '{ev.Title}'");

        return ResultResponse.Success("Event deleted successfully.");
    }

    private static EventDto MapToDto(Event e)
    {
        return new EventDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            EventDate = e.EventDate,
            Location = e.Location,
            BannerImageUrl = e.BannerImageUrl,
            IsPublic = e.IsPublic,
            IsUpcoming = e.IsUpcoming
        };
    }
}

public class GalleryService : IGalleryService
{
    private readonly ApplicationDbContext _context;
    private readonly IAccountService _accountService;

    public GalleryService(ApplicationDbContext context, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<IReadOnlyList<GalleryDto>> GetActiveGalleryAsync(string? category = null)
    {
        var query = _context.Galleries.Where(g => g.IsActive);
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(g => g.Category.ToLower() == category.Trim().ToLower());
        }

        var items = await query.OrderByDescending(g => g.CreatedAt).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<GalleryDto>> GetAllGalleryAsync()
    {
        var items = await _context.Galleries.OrderByDescending(g => g.CreatedAt).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<GalleryDto?> GetByIdAsync(int id)
    {
        var g = await _context.Galleries.FindAsync(id);
        return g == null ? null : MapToDto(g);
    }

    public async Task<ResultResponse<GalleryDto>> CreateAsync(GalleryDto dto, string? currentUserId = null)
    {
        var gallery = new Gallery
        {
            Title = dto.Title.Trim(),
            Category = dto.Category.Trim(),
            ImageUrl = dto.ImageUrl,
            Caption = dto.Caption?.Trim(),
            EventDate = dto.EventDate,
            IsActive = dto.IsActive
        };

        await _context.Galleries.AddAsync(gallery);
        await _context.SaveChangesAsync();
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Gallery", gallery.Id.ToString(), $"Uploaded gallery photo '{gallery.Title}'");

        return ResultResponse<GalleryDto>.Success(MapToDto(gallery), "Gallery item saved successfully.");
    }

    public async Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null)
    {
        var g = await _context.Galleries.FindAsync(id);
        if (g == null)
            return ResultResponse.Failure("Gallery item not found.");

        g.IsDeleted = true;
        await _context.SaveChangesAsync();
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Gallery", id.ToString(), "Deleted gallery item");

        return ResultResponse.Success("Gallery item removed successfully.");
    }

    private static GalleryDto MapToDto(Gallery g)
    {
        return new GalleryDto
        {
            Id = g.Id,
            Title = g.Title,
            Category = g.Category,
            ImageUrl = g.ImageUrl,
            Caption = g.Caption,
            EventDate = g.EventDate,
            IsActive = g.IsActive
        };
    }
}

