using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class SchoolClass : AuditableEntity
{
    public string Name { get; set; } = string.Empty; // e.g. "Class 6", "Class 10"
    public int NumericValue { get; set; } // e.g. 6, 10
    public string? Description { get; set; }

    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<ClassRoutine> Routines { get; set; } = new List<ClassRoutine>();
}

