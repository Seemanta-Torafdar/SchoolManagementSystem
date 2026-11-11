using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Section : AuditableEntity
{
    public string Name { get; set; } = string.Empty; // e.g. "A", "B", "Rose", "Padma"
    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }
    public string? RoomNumber { get; set; }
    public int Capacity { get; set; } = 40;

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<ClassRoutine> Routines { get; set; } = new List<ClassRoutine>();
}

