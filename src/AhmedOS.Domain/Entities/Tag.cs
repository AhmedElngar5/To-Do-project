using AhmedOS.Domain.Common;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Reusable tag for cross-entity labeling.
/// </summary>
public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ICollection<TaskTag> TaskTags { get; set; } = [];
    public ICollection<NoteTag> NoteTags { get; set; } = [];
}
