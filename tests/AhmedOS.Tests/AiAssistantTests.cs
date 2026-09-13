using AhmedOS.Infrastructure.Services;
using Xunit;

namespace AhmedOS.Tests;

public class AiAssistantTests
{
    private readonly LocalAiService _ai;

    public AiAssistantTests()
    {
        _ai = new LocalAiService(null!);
    }

    [Fact]
    public async Task DecomposeGoal_ShouldReturnActionableMilestones()
    {
        var steps = await _ai.DecomposeGoalAsync("Build Facial Recognition System");

        Assert.NotNull(steps);
        Assert.True(steps.Count >= 4);
        Assert.Contains(steps, s => s.Contains("Build Facial Recognition System"));
    }

    [Fact]
    public async Task DecomposeTask_ShouldReturnDetailedSubtasks()
    {
        var subtasks = await _ai.DecomposeTaskAsync("Implement JWT Auth");

        Assert.NotNull(subtasks);
        Assert.True(subtasks.Count >= 3);
    }

    [Fact]
    public async Task ParseQuickCapture_ShouldTrimAndFormat()
    {
        var result = await _ai.ParseQuickCaptureAsync("   Finish graduation report   ");

        Assert.Equal("Finish graduation report", result);
    }
}
