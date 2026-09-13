using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Project — container for related tasks, milestones, and notes.
/// </summary>
public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectType Type { get; set; } = ProjectType.Personal;
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public ProjectHealth Health { get; set; } = ProjectHealth.Healthy;
    public Priority Priority { get; set; } = Priority.Medium;

    // Dates
    public DateTime? StartDate { get; set; }
    public DateTime? Deadline { get; set; }

    // Progress (0-100)
    public int Progress { get; set; }

    // Project metadata
    public string? RepositoryUrl { get; set; }
    public string? LiveUrl { get; set; }
    public string? Technologies { get; set; } // Comma-separated
    public string? Color { get; set; }
    public string? Icon { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;
    public int? CategoryId { get; set; }

    // Navigation
    public Category? Category { get; set; }
    public ICollection<TodoTask> Tasks { get; set; } = [];
    public ICollection<ProjectMilestone> Milestones { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<TimeEntry> TimeEntries { get; set; } = [];
}

/// <summary>
/// Milestone within a project.
/// </summary>
public class ProjectMilestone : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int SortOrder { get; set; }

    // Foreign keys
    public int ProjectId { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public Project Project { get; set; } = null!;
}
