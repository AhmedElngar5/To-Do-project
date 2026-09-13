using AhmedOS.Domain.Common;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// University course.
/// </summary>
public class Course : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Instructor { get; set; }
    public int? CreditHours { get; set; }
    public string? Semester { get; set; }
    public int Progress { get; set; } // 0-100
    public string? Color { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ICollection<TodoTask> Tasks { get; set; } = [];
    public ICollection<CourseGrade> Grades { get; set; } = [];
    public ICollection<CalendarEvent> Events { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
}

/// <summary>
/// Grade entry for a course.
/// </summary>
public class CourseGrade : BaseEntity
{
    public string Name { get; set; } = string.Empty; // "Midterm", "Final", "Quiz 1"
    public string? Type { get; set; } // Quiz, Midterm, Final, Assignment, Lab, Project
    public double? MaxScore { get; set; }
    public double? Score { get; set; }
    public double? Weight { get; set; } // Percentage weight
    public DateTime? Date { get; set; }

    public int CourseId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Course Course { get; set; } = null!;
}
