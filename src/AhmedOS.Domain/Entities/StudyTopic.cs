using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Study topic — tracks learning progress and mastery.
/// </summary>
public class StudyTopic : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public StudyStatus Status { get; set; } = StudyStatus.NotStarted;
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    public ConfidenceLevel Confidence { get; set; } = ConfidenceLevel.None;
    public int Progress { get; set; } // 0-100

    // Revision tracking
    public DateTime? LastStudied { get; set; }
    public DateTime? NextReview { get; set; }
    public int ReviewCount { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;
    public int? CategoryId { get; set; }

    // Navigation
    public Category? Category { get; set; }
    public ICollection<TodoTask> Tasks { get; set; } = [];
    public ICollection<StudySession> StudySessions { get; set; } = [];
    public ICollection<StudyResource> Resources { get; set; } = [];
}

/// <summary>
/// Study session — tracks individual study periods.
/// </summary>
public class StudySession : BaseEntity
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public ConfidenceLevel? ConfidenceAfter { get; set; }

    public int StudyTopicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public StudyTopic StudyTopic { get; set; } = null!;
}

/// <summary>
/// Resource linked to a study topic.
/// </summary>
public class StudyResource : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Type { get; set; } // Video, Article, Book, Documentation, etc.
    public bool IsCompleted { get; set; }

    public int StudyTopicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public StudyTopic StudyTopic { get; set; } = null!;
}
