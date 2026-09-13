using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Skill in the roadmap toward Full Stack .NET Developer.
/// </summary>
public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public StudyStatus Status { get; set; } = StudyStatus.NotStarted;
    public ConfidenceLevel Confidence { get; set; } = ConfidenceLevel.None;
    public int Progress { get; set; } // 0-100
    public int SortOrder { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;
    public int? ParentSkillId { get; set; }

    // Navigation
    public Skill? ParentSkill { get; set; }
    public ICollection<Skill> SubSkills { get; set; } = [];
}
