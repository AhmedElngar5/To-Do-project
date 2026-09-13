using AhmedOS.Domain.Common;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// DEPI training module.
/// </summary>
public class DEPIModule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Progress { get; set; } // 0-100
    public bool IsCompleted { get; set; }
    public int SortOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ICollection<TodoTask> Tasks { get; set; } = [];
    public ICollection<DEPISession> Sessions { get; set; } = [];
}

/// <summary>
/// Individual DEPI training session.
/// </summary>
public class DEPISession : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime? SessionDate { get; set; }
    public int? DurationMinutes { get; set; }
    public bool IsAttended { get; set; }

    public int DEPIModuleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DEPIModule DEPIModule { get; set; } = null!;
}
