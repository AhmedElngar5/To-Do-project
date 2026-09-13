using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// In-app notification.
/// </summary>
public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; } // Link to related page
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }

    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// Audit log entry for important changes.
/// </summary>
public class AuditLog : BaseEntity
{
    public string Action { get; set; } = string.Empty; // Created, Updated, Deleted, Completed, etc.
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string? EntityTitle { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// User settings/preferences.
/// </summary>
public class UserSettings : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    
    // Work preferences
    public string? WorkStartTime { get; set; } = "09:00";
    public string? WorkEndTime { get; set; } = "17:00";
    public int MaxDeepWorkHours { get; set; } = 4;
    public int MaxStudyHours { get; set; } = 3;
    public int DefaultTaskDurationMinutes { get; set; } = 30;
    public int DefaultBreakMinutes { get; set; } = 10;

    // Preferences
    public string Theme { get; set; } = "dark";
    public string Language { get; set; } = "en";
    public string Timezone { get; set; } = "Africa/Cairo";
    public int WeekStartDay { get; set; } = 0; // 0 = Sunday
    
    // Energy preferences
    public TimeOfDay PreferredDeepWorkTime { get; set; } = TimeOfDay.Morning;
    public TimeOfDay PreferredStudyTime { get; set; } = TimeOfDay.Morning;

    // Notification preferences
    public bool NotificationsEnabled { get; set; } = true;
    public int DefaultReminderMinutes { get; set; } = 15;

    // Dashboard layout (JSON)
    public string? DashboardLayout { get; set; }
}

/// <summary>
/// Daily review entry.
/// </summary>
public class DailyReview : BaseEntity
{
    public DateTime Date { get; set; }
    public string? Accomplishments { get; set; }
    public string? Challenges { get; set; }
    public string? Notes { get; set; }
    public int? MoodRating { get; set; } // 1-5
    public int? ProductivityRating { get; set; } // 1-5
    public string? TomorrowPlan { get; set; }

    public string UserId { get; set; } = string.Empty;
}
