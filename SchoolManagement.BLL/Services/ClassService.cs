using SchoolManagement.BLL.DTOs.Class;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.BLL.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository _classRepo;
    private readonly IAccountService _accountService;

    public ClassService(IClassRepository classRepo, IAccountService accountService)
    {
        _classRepo = classRepo;
        _accountService = accountService;
    }

    public async Task<IReadOnlyList<ClassDto>> GetAllClassesAsync()
    {
        var classes = await _classRepo.GetAllClassesWithSectionsAsync();
        return classes.Select(c => new ClassDto
        {
            Id = c.Id,
            Name = c.Name,
            NumericValue = c.NumericValue,
            Description = c.Description,
            StudentCount = c.Students.Count,
            Sections = c.Sections.Select(s => new SectionDto
            {
                Id = s.Id,
                SchoolClassId = s.SchoolClassId,
                ClassName = c.Name,
                Name = s.Name,
                RoomNumber = s.RoomNumber,
                Capacity = s.Capacity,
                StudentCount = s.Students.Count
            }).ToList(),
            SubjectNames = c.Subjects.Select(sub => sub.Name).ToList()
        }).ToList();
    }

    public async Task<ClassDto?> GetByIdAsync(int id)
    {
        var c = await _classRepo.GetClassWithDetailsAsync(id);
        if (c == null) return null;

        return new ClassDto
        {
            Id = c.Id,
            Name = c.Name,
            NumericValue = c.NumericValue,
            Description = c.Description,
            StudentCount = c.Students.Count,
            Sections = c.Sections.Select(s => new SectionDto
            {
                Id = s.Id,
                SchoolClassId = s.SchoolClassId,
                ClassName = c.Name,
                Name = s.Name,
                RoomNumber = s.RoomNumber,
                Capacity = s.Capacity,
                StudentCount = s.Students.Count
            }).ToList(),
            SubjectNames = c.Subjects.Select(sub => sub.Name).ToList()
        };
    }

    public async Task<ResultResponse<ClassDto>> CreateClassAsync(CreateClassDto dto, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return ResultResponse<ClassDto>.Failure("Class name is required.");

        var schoolClass = new SchoolClass
        {
            Name = dto.Name.Trim(),
            NumericValue = dto.NumericValue,
            Description = dto.Description?.Trim()
        };

        var created = await _classRepo.AddAsync(schoolClass);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Class", created.Id.ToString(), $"Created class {created.Name}");

        return ResultResponse<ClassDto>.Success(new ClassDto
        {
            Id = created.Id,
            Name = created.Name,
            NumericValue = created.NumericValue,
            Description = created.Description
        }, "Class created successfully.");
    }

    public async Task<ResultResponse> UpdateClassAsync(int id, CreateClassDto dto, string? currentUserId = null)
    {
        var schoolClass = await _classRepo.GetByIdAsync(id);
        if (schoolClass == null)
            return ResultResponse.Failure("Class not found.");

        schoolClass.Name = dto.Name.Trim();
        schoolClass.NumericValue = dto.NumericValue;
        schoolClass.Description = dto.Description?.Trim();

        await _classRepo.UpdateAsync(schoolClass);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "Class", id.ToString(), $"Updated class {schoolClass.Name}");

        return ResultResponse.Success("Class updated successfully.");
    }

    public async Task<ResultResponse> DeleteClassAsync(int id, string? currentUserId = null)
    {
        var schoolClass = await _classRepo.GetClassWithDetailsAsync(id);
        if (schoolClass == null)
            return ResultResponse.Failure("Class not found.");

        if (schoolClass.Students.Any())
            return ResultResponse.Failure("Cannot delete class with enrolled students. Reassign or remove students first.");

        await _classRepo.DeleteAsync(schoolClass);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Class", id.ToString(), $"Deleted class {schoolClass.Name}");

        return ResultResponse.Success("Class deleted successfully.");
    }

    public async Task<IReadOnlyList<SectionDto>> GetSectionsByClassIdAsync(int classId)
    {
        var sections = await _classRepo.GetSectionsByClassIdAsync(classId);
        return sections.Select(s => new SectionDto
        {
            Id = s.Id,
            SchoolClassId = s.SchoolClassId,
            ClassName = s.SchoolClass?.Name ?? "",
            Name = s.Name,
            RoomNumber = s.RoomNumber,
            Capacity = s.Capacity,
            StudentCount = s.Students.Count
        }).ToList();
    }

    public async Task<ResultResponse<SectionDto>> CreateSectionAsync(CreateSectionDto dto, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return ResultResponse<SectionDto>.Failure("Section name is required.");

        var existing = await _classRepo.GetSectionsByClassIdAsync(dto.SchoolClassId);
        if (existing.Any(s => s.Name.Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return ResultResponse<SectionDto>.Failure("A section with this name already exists in this class.");

        var section = new Section
        {
            SchoolClassId = dto.SchoolClassId,
            Name = dto.Name.Trim(),
            RoomNumber = dto.RoomNumber?.Trim(),
            Capacity = dto.Capacity
        };

        var created = await _classRepo.AddSectionAsync(section);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Section", created.Id.ToString(), $"Created section {created.Name} for class #{created.SchoolClassId}");

        return ResultResponse<SectionDto>.Success(new SectionDto
        {
            Id = created.Id,
            SchoolClassId = created.SchoolClassId,
            Name = created.Name,
            RoomNumber = created.RoomNumber,
            Capacity = created.Capacity
        }, "Section created successfully.");
    }

    public async Task<ResultResponse> DeleteSectionAsync(int id, string? currentUserId = null)
    {
        var section = await _classRepo.GetSectionByIdAsync(id);
        if (section == null)
            return ResultResponse.Failure("Section not found.");

        if (section.Students.Any())
            return ResultResponse.Failure("Cannot delete section with active students.");

        await _classRepo.DeleteSectionAsync(section);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Section", id.ToString(), $"Deleted section {section.Name}");

        return ResultResponse.Success("Section deleted successfully.");
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _classRepo.CountAsync();
    }
}

