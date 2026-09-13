using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Note — lightweight personal knowledge base item.
/// </summary>
public class Note : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; } // Markdown content
    public NoteType Type { get; set; } = NoteType.Quick;
    public bool IsPinned { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsArchived { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public int? ProjectId { get; set; }
    public int? CourseId { get; set; }
    public int? GoalId { get; set; }

    // Navigation
    public Category? Category { get; set; }
    public Project? Project { get; set; }
    public Course? Course { get; set; }
    public Goal? Goal { get; set; }
    public ICollection<NoteTag> NoteTags { get; set; } = [];
}
