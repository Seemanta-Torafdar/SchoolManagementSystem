using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Interfaces;

public interface IClassRepository : IGenericRepository<SchoolClass>
{
    Task<SchoolClass?> GetClassWithDetailsAsync(int id);
    Task<IReadOnlyList<SchoolClass>> GetAllClassesWithSectionsAsync();
    Task<Section?> GetSectionByIdAsync(int sectionId);
    Task<IReadOnlyList<Section>> GetSectionsByClassIdAsync(int classId);
    Task<Section> AddSectionAsync(Section section);
    Task UpdateSectionAsync(Section section);
    Task DeleteSectionAsync(Section section);
}

