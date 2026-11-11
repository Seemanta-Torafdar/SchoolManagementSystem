namespace SchoolManagement.BLL.DTOs.Event;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? BannerImageUrl { get; set; }
    public bool IsPublic { get; set; }
    public bool IsUpcoming { get; set; }
}

public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public string Location { get; set; } = "School Auditorium";
    public string? BannerImageUrl { get; set; }
    public bool IsPublic { get; set; } = true;
}

