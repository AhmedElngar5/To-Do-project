using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using Xunit;

namespace AhmedOS.Tests;

public class WaselProjectTests
{
    [Fact]
    public void WaselProject_Initialization_ShouldMatchGraduationRequirements()
    {
        var project = new Project
        {
            Name = "Wasel",
            Description = "AI-Based Child Family Reunification & Alternative Care Support System",
            Type = ProjectType.Graduation,
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Healthy,
            Priority = Priority.Critical,
            Technologies = "C#, ASP.NET Core 10, SQL Server, Angular, AI, NLP, Computer Vision"
        };

        Assert.Equal("Wasel", project.Name);
        Assert.Equal(ProjectType.Graduation, project.Type);
        Assert.Equal(Priority.Critical, project.Priority);
        Assert.Contains("ASP.NET Core 10", project.Technologies);
        Assert.Contains("Computer Vision", project.Technologies);
    }

    [Fact]
    public void WaselMilestones_ShouldTrackCompletion()
    {
        var milestone = new ProjectMilestone
        {
            Title = "Face Recognition & Biometric Matching Pipeline",
            IsCompleted = false,
            SortOrder = 3
        };

        Assert.False(milestone.IsCompleted);
        Assert.Null(milestone.CompletedAt);

        milestone.IsCompleted = true;
        milestone.CompletedAt = DateTime.UtcNow;

        Assert.True(milestone.IsCompleted);
        Assert.NotNull(milestone.CompletedAt);
    }
}
