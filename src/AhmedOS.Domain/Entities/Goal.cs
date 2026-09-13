using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Goal with hierarchy support (Vision → Year → Quarter → Month → Week → Day).
/// </summary>
public class Goal : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WhyItMatters { get; set; }
    public GoalLevel Level { get; set; } = GoalLevel.Yearly;
    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
    public Priority Priority { get; set; } = Priority.Medium;

    // Progress (0-100)
    public int Progress { get; set; }

    // Dates
    public DateTime? StartDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Hierarchy
    public int? ParentGoalId { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public Goal? ParentGoal { get; set; }
    public ICollection<Goal> SubGoals { get; set; } = [];
    public ICollection<TodoTask> Tasks { get; set; } = [];
    public ICollection<GoalMilestone> Milestones { get; set; } = [];
}

/// <summary>
/// Milestone within a goal.
/// </summary>
public class GoalMilestone : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int SortOrder { get; set; }

    public int GoalId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Goal Goal { get; set; } = null!;
}
