using SchoolManagement.BLL.DTOs.Routine;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.BLL.Services;

public class RoutineService : IRoutineService
{
    private readonly IRoutineRepository _routineRepo;
    private readonly IAccountService _accountService;

    public RoutineService(IRoutineRepository routineRepo, IAccountService accountService)
    {
        _routineRepo = routineRepo;
        _accountService = accountService;
    }

    public async Task<IReadOnlyList<RoutineDto>> GetRoutineByClassAndSectionAsync(int classId, int sectionId)
    {
        var items = await _routineRepo.GetRoutineByClassAndSectionAsync(classId, sectionId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<RoutineDto>> GetRoutineByTeacherAsync(int teacherId)
    {
        var items = await _routineRepo.GetRoutineByTeacherAsync(teacherId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<RoutineDto?> GetByIdAsync(int id)
    {
        var r = await _routineRepo.GetByIdAsync(id);
        return r == null ? null : MapToDto(r);
    }

    public async Task<ResultResponse<RoutineDto>> CreateAsync(CreateRoutineDto dto, string? currentUserId = null)
    {
        // Check for schedule conflict
        var hasConflict = await _routineRepo.HasConflictAsync(dto.DayOfWeek, dto.PeriodNumber, dto.TeacherId, dto.SchoolClassId, dto.SectionId);
        if (hasConflict)
            return ResultResponse<RoutineDto>.Failure("Routine schedule conflict detected! Either the Teacher or Section is already assigned for this day and period.");

        var routine = new ClassRoutine
        {
            SchoolClassId = dto.SchoolClassId,
            SectionId = dto.SectionId,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            DayOfWeek = dto.DayOfWeek,
            PeriodNumber = dto.PeriodNumber,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            RoomNumber = dto.RoomNumber?.Trim()
        };

        var created = await _routineRepo.AddAsync(routine);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "ClassRoutine", created.Id.ToString(), $"Scheduled period {created.PeriodNumber} ({created.DayOfWeek})");

        return ResultResponse<RoutineDto>.Success(MapToDto(created), "Class routine scheduled successfully.");
    }

    public async Task<ResultResponse> UpdateAsync(int id, CreateRoutineDto dto, string? currentUserId = null)
    {
        var routine = await _routineRepo.GetByIdAsync(id);
        if (routine == null)
            return ResultResponse.Failure("Routine entry not found.");

        var hasConflict = await _routineRepo.HasConflictAsync(dto.DayOfWeek, dto.PeriodNumber, dto.TeacherId, dto.SchoolClassId, dto.SectionId, id);
        if (hasConflict)
            return ResultResponse.Failure("Schedule conflict detected for the selected time slot.");

        routine.SchoolClassId = dto.SchoolClassId;
        routine.SectionId = dto.SectionId;
        routine.SubjectId = dto.SubjectId;
        routine.TeacherId = dto.TeacherId;
        routine.DayOfWeek = dto.DayOfWeek;
        routine.PeriodNumber = dto.PeriodNumber;
        routine.StartTime = dto.StartTime;
        routine.EndTime = dto.EndTime;
        routine.RoomNumber = dto.RoomNumber?.Trim();

        await _routineRepo.UpdateAsync(routine);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "ClassRoutine", id.ToString(), $"Updated routine period {routine.PeriodNumber}");

        return ResultResponse.Success("Routine entry updated successfully.");
    }

    public async Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null)
    {
        var routine = await _routineRepo.GetByIdAsync(id);
        if (routine == null)
            return ResultResponse.Failure("Routine entry not found.");

        await _routineRepo.DeleteAsync(routine);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "ClassRoutine", id.ToString(), "Deleted routine entry");

        return ResultResponse.Success("Routine entry deleted successfully.");
    }

    private static RoutineDto MapToDto(ClassRoutine r)
    {
        return new RoutineDto
        {
            Id = r.Id,
            SchoolClassId = r.SchoolClassId,
            ClassName = r.SchoolClass?.Name ?? $"Class #{r.SchoolClassId}",
            SectionId = r.SectionId,
            SectionName = r.Section?.Name ?? $"Section #{r.SectionId}",
            SubjectId = r.SubjectId,
            SubjectName = r.Subject?.Name ?? $"Subject #{r.SubjectId}",
            TeacherId = r.TeacherId,
            TeacherName = r.Teacher?.FullName ?? $"Teacher #{r.TeacherId}",
            DayOfWeek = r.DayOfWeek,
            PeriodNumber = r.PeriodNumber,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            RoomNumber = r.RoomNumber
        };
    }
}

