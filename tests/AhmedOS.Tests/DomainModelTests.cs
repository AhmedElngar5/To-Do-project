using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using Xunit;

namespace AhmedOS.Tests;

public class DomainModelTests
{
    [Fact]
    public void TodoTask_DefaultValues_ShouldBeSane()
    {
        var task = new TodoTask();

        Assert.Equal(string.Empty, task.Title);
        Assert.Equal(TodoTaskStatus.Inbox, task.Status);
        Assert.Equal(Priority.Medium, task.Priority);
        Assert.Equal(RecurrenceType.None, task.RecurrenceType);
        Assert.False(task.IsDeleted);
        Assert.NotNull(task.SubTasks);
        Assert.Empty(task.SubTasks);
    }

    [Fact]
    public void Goal_DefaultLevelAndStatus_ShouldBeValid()
    {
        var goal = new Goal
        {
            Title = "Master .NET Architecture",
            Level = GoalLevel.Yearly
        };

        Assert.Equal("Master .NET Architecture", goal.Title);
        Assert.Equal(GoalLevel.Yearly, goal.Level);
        Assert.Equal(GoalStatus.NotStarted, goal.Status);
        Assert.Equal(0, goal.Progress);
    }

    [Fact]
    public void Habit_CurrentStreak_ShouldBeInitialized()
    {
        var habit = new Habit
        {
            Name = "Morning Exercise",
            Schedule = RecurrenceType.Daily
        };

        Assert.Equal("Morning Exercise", habit.Name);
        Assert.Equal(0, habit.CurrentStreak);
        Assert.Equal(0, habit.BestStreak);
        Assert.True(habit.IsActive);
    }

    [Fact]
    public void UserSettings_DefaultTimes_ShouldBeConfigured()
    {
        var settings = new UserSettings();

        Assert.Equal("dark", settings.Theme);
        Assert.Equal("09:00", settings.WorkStartTime);
        Assert.Equal("17:00", settings.WorkEndTime);
        Assert.Equal(4, settings.MaxDeepWorkHours);
    }
}
