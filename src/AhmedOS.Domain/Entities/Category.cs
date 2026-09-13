using AhmedOS.Domain.Common;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Reusable category for organizing tasks, projects, notes, etc.
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; } // System-provided categories cannot be deleted
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ICollection<TodoTask> Tasks { get; set; } = [];
    public ICollection<Project> Projects { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
}
