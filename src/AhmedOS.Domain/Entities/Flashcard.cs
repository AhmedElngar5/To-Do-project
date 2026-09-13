using AhmedOS.Domain.Common;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Spaced repetition flashcard for study topics.
/// </summary>
public class Flashcard : BaseEntity
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public int Box { get; set; } = 1; // Leitner box (1 to 5)
    public DateTime? NextReviewDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastReviewedAt { get; set; }
    public int ReviewCount { get; set; }

    public int StudyTopicId { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public StudyTopic? StudyTopic { get; set; }
}
