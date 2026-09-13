using AhmedOS.Application.Interfaces;

namespace AhmedOS.Infrastructure.Services;

/// <summary>
/// Placeholder AI service when no AI provider is configured.
/// Uses deterministic logic instead of AI for basic operations.
/// </summary>
public class LocalAiService : IAiService
{
    public bool IsConfigured => false;

    public Task<string> GetDailyBriefingAsync(string userId)
        => Task.FromResult("AI briefing is not configured. Your daily summary is generated from your task and schedule data.");

    public Task<string> GetNextActionRecommendationAsync(string userId)
        => Task.FromResult("AI recommendations require an AI provider to be configured.");

    public Task<List<string>> DecomposeGoalAsync(string goalDescription)
        => Task.FromResult(new List<string> { "AI decomposition requires an AI provider to be configured. Break this goal down manually." });

    public Task<List<string>> DecomposeTaskAsync(string taskDescription)
        => Task.FromResult(new List<string> { "AI decomposition requires an AI provider to be configured. Break this task down manually." });

    public Task<string> ParseQuickCaptureAsync(string input)
        => Task.FromResult(input); // Pass-through without AI parsing

    public Task<string> ChatAsync(string userId, string message)
        => Task.FromResult("AI Assistant is not configured. Add an AI provider API key in Settings to enable this feature.");
}
