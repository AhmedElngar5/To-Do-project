using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Habit definition with schedule configuration.
/// </summary>
public class Habit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }

    // Schedule
    public RecurrenceType Schedule { get; set; } = RecurrenceType.Daily;
    public string? ScheduleDays { get; set; } // JSON array of days, e.g. ["Mon","Wed","Fri"]
    public int? TargetCount { get; set; } // e.g., drink 8 glasses
    public string? TargetUnit { get; set; } // e.g., "glasses", "minutes"

    // Stats (denormalized for performance)
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public int TotalCompletions { get; set; }

    public bool IsActive { get; set; } = true;

    // Foreign keys
    public string UserId { get; set; } = string.Empty;
    public int? CategoryId { get; set; }

    // Navigation
    public Category? Category { get; set; }
    public ICollection<HabitEntry> Entries { get; set; } = [];
}

/// <summary>
/// Daily habit completion entry.
/// </summary>
public class HabitEntry : BaseEntity
{
    public DateTime Date { get; set; }
    public bool IsCompleted { get; set; }
    public int? Count { get; set; } // For counted habits
    public string? Notes { get; set; }

    public int HabitId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Habit Habit { get; set; } = null!;
}
