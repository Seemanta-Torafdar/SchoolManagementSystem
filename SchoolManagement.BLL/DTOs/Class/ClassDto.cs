namespace SchoolManagement.BLL.DTOs.Class;

public class ClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NumericValue { get; set; }
    public string? Description { get; set; }
    public int StudentCount { get; set; }
    public List<SectionDto> Sections { get; set; } = new();
    public List<string> SubjectNames { get; set; } = new();
}

public class CreateClassDto
{
    public string Name { get; set; } = string.Empty;
    public int NumericValue { get; set; }
    public string? Description { get; set; }
}

public class SectionDto
{
    public int Id { get; set; }
    public int SchoolClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? RoomNumber { get; set; }
    public int Capacity { get; set; }
    public int StudentCount { get; set; }
}

public class CreateSectionDto
{
    public int SchoolClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? RoomNumber { get; set; }
    public int Capacity { get; set; } = 40;
}

