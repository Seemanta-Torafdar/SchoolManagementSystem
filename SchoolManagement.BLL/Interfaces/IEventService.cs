using SchoolManagement.BLL.DTOs.Event;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IEventService
{
    Task<IReadOnlyList<EventDto>> GetUpcomingEventsAsync(int count = 6);
    Task<IReadOnlyList<EventDto>> GetAllEventsAsync();
    Task<EventDto?> GetByIdAsync(int id);
    Task<ResultResponse<EventDto>> CreateAsync(CreateEventDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateAsync(int id, CreateEventDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null);
}
