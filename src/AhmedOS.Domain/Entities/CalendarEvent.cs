using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Calendar event.
/// </summary>
public class CalendarEvent : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CalendarEventType EventType { get; set; } = CalendarEventType.Personal;
    public string? Color { get; set; }

    // Time
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAllDay { get; set; }

    // Recurrence
    public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
    public string? RecurrenceRule { get; set; }

    // Location
    public string? Location { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public int? CourseId { get; set; }

    // Navigation
    public Project? Project { get; set; }
    public Course? Course { get; set; }
}
