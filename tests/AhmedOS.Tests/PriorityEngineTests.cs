using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Services;
using Xunit;

namespace AhmedOS.Tests;

public class PriorityEngineTests
{
    private readonly PriorityEngine _engine;

    public PriorityEngineTests()
    {
        // For pure scoring calculation, DbContext is not queried
        _engine = new PriorityEngine(null!);
    }

    [Fact]
    public void CriticalPriority_ShouldHaveHighBaseScore()
    {
        var task = new TodoTask
        {
            Title = "Critical Bug Fix",
            Priority = Priority.Critical
        };

        var (score, reason) = _engine.CalculatePriorityScore(task);

        Assert.True(score >= 40);
        Assert.Contains("Critical", reason);
    }

    [Fact]
    public void OverdueTask_ShouldAddUrgencyPoints()
    {
        var task = new TodoTask
        {
            Title = "Overdue Assignment",
            Priority = Priority.High,
            DueDate = DateTime.UtcNow.AddDays(-2)
        };

        var (score, reason) = _engine.CalculatePriorityScore(task);

        Assert.True(score >= 65);
        Assert.Contains("Overdue", reason);
    }

    [Fact]
    public void StrategicGoalAndCourse_ShouldBoostScore()
    {
        var task = new TodoTask
        {
            Title = "Graduation Milestone",
            Priority = Priority.High,
            GoalId = 1,
            ProjectId = 2,
            CourseId = 3
        };

        var (score, reason) = _engine.CalculatePriorityScore(task);

        Assert.True(score >= 60);
        Assert.Contains("Goal", reason);
        Assert.Contains("Project", reason);
        Assert.Contains("course", reason);
    }

    [Fact]
    public void BlockedTask_ShouldReceivePenalty()
    {
        var normalTask = new TodoTask
        {
            Title = "Regular Task",
            Priority = Priority.High
        };

        var blockedTask = new TodoTask
        {
            Title = "Regular Task",
            Priority = Priority.High,
            Status = TodoTaskStatus.Blocked
        };

        var (normalScore, _) = _engine.CalculatePriorityScore(normalTask);
        var (blockedScore, blockedReason) = _engine.CalculatePriorityScore(blockedTask);

        Assert.True(blockedScore < normalScore);
        Assert.Contains("Blocked", blockedReason);
    }

    [Fact]
    public void Score_ShouldNeverExceed100()
    {
        var extremeTask = new TodoTask
        {
            Title = "Max Overload Task",
            Priority = Priority.Critical,
            DueDate = DateTime.UtcNow.AddDays(-5),
            GoalId = 1,
            ProjectId = 1,
            CourseId = 1,
            DEPIModuleId = 1,
            Status = TodoTaskStatus.InProgress
        };

        var (score, _) = _engine.CalculatePriorityScore(extremeTask);

        Assert.InRange(score, 0, 100);
        Assert.Equal(100, score);
    }
}
