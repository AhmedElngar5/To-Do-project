using AhmedOS.Domain.Common;

namespace AhmedOS.Domain.Entities;

/// <summary>Many-to-many relationship between TodoTask and Tag.</summary>
public class TaskTag
{
    public int TaskId { get; set; }
    public int TagId { get; set; }
    public TodoTask Task { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}

/// <summary>Reminder on a task.</summary>
public class TaskReminder : BaseEntity
{
    public int TaskId { get; set; }
    public DateTime RemindAt { get; set; }
    public bool IsSent { get; set; }
    public string UserId { get; set; } = string.Empty;
    public TodoTask Task { get; set; } = null!;
}

/// <summary>Many-to-many relationship between Note and Tag.</summary>
public class NoteTag
{
    public int NoteId { get; set; }
    public int TagId { get; set; }
    public Note Note { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
