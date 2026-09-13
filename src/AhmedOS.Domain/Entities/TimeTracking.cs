using AhmedOS.Domain.Common;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Focus/Pomodoro session.
/// </summary>
public class FocusSession : BaseEntity
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int PlannedMinutes { get; set; } = 25;
    public int? ActualMinutes { get; set; }
    public bool IsCompleted { get; set; }

    // Link to what was being worked on
    public int? TaskId { get; set; }
    public int? ProjectId { get; set; }
    public int? StudyTopicId { get; set; }

    public string UserId { get; set; } = string.Empty;

    // Navigation
    public TodoTask? Task { get; set; }
    public Project? Project { get; set; }
    public StudyTopic? StudyTopic { get; set; }
}

/// <summary>
/// Time entry for tracking time spent.
/// </summary>
public class TimeEntry : BaseEntity
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string? Description { get; set; }

    // Link to entity
    public int? TaskId { get; set; }
    public int? ProjectId { get; set; }
    public int? StudyTopicId { get; set; }

    public string UserId { get; set; } = string.Empty;

    // Navigation
    public TodoTask? Task { get; set; }
    public Project? Project { get; set; }
    public StudyTopic? StudyTopic { get; set; }
}
