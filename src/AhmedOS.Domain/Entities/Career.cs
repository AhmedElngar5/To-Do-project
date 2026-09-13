using AhmedOS.Domain.Common;
using AhmedOS.Domain.Enums;

namespace AhmedOS.Domain.Entities;

/// <summary>
/// Company — reusable across multiple job applications.
/// </summary>
public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ICollection<JobApplication> Applications { get; set; } = [];
}

/// <summary>
/// Job application in the career pipeline.
/// </summary>
public class JobApplication : BaseEntity
{
    public string Position { get; set; } = string.Empty;
    public string? Location { get; set; }
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Saved;
    public string? Url { get; set; }
    public string? Source { get; set; } // LinkedIn, Indeed, etc.
    public DateTime? ApplicationDate { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? Salary { get; set; }
    public string? Notes { get; set; }

    // Foreign keys
    public int? CompanyId { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public Company? Company { get; set; }
    public ICollection<JobInterview> Interviews { get; set; } = [];
}

/// <summary>
/// Interview for a job application.
/// </summary>
public class JobInterview : BaseEntity
{
    public DateTime ScheduledAt { get; set; }
    public InterviewType Type { get; set; } = InterviewType.Technical;
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public string? Notes { get; set; }
    public string? PreparationNotes { get; set; }
    public string? Location { get; set; }

    public int JobApplicationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public JobApplication JobApplication { get; set; } = null!;
}
