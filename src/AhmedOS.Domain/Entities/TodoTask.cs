using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Core task entity — the most important entity in the system.
/// Named TodoTask to avoid conflict with System.Threading.Tasks.Task.
/// </summary>
public class TodoTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoTaskStatus Status { get; set; } = TodoTaskStatus.Inbox;
    public Priority Priority { get; set; } = Priority.Medium;
    public EnergyLevel? EnergyLevel { get; set; }
    public TaskContext? Context { get; set; }

    // Dates
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Duration
    public int? EstimatedMinutes { get; set; }
    public int? ActualMinutes { get; set; }

    // Recurrence
    public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
    public string? RecurrenceRule { get; set; } // JSON-encoded custom rules

    // Sorting
    public int SortOrder { get; set; }

    // Source (where this task came from: inbox, ai, routine, etc.)
    public string? Source { get; set; }

    // Priority score (computed by the engine)
    public double? PriorityScore { get; set; }
    public string? PriorityReason { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public int? ProjectId { get; set; }
    public int? GoalId { get; set; }
    public int? CourseId { get; set; }
    public int? DEPIModuleId { get; set; }
    public int? StudyTopicId { get; set; }
    public int? ParentTaskId { get; set; }

    // Navigation
    public Category? Category { get; set; }
    public Project? Project { get; set; }
    public Goal? Goal { get; set; }
    public Course? Course { get; set; }
    public DEPIModule? DEPIModule { get; set; }
    public StudyTopic? StudyTopic { get; set; }
    public TodoTask? ParentTask { get; set; }
    public ICollection<TodoTask> SubTasks { get; set; } = [];
    public ICollection<TaskTag> TaskTags { get; set; } = [];
    public ICollection<TimeEntry> TimeEntries { get; set; } = [];
    public ICollection<TaskReminder> Reminders { get; set; } = [];
}
