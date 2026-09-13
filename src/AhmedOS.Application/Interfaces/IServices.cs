namespace AhmedOS.Application.Interfaces;

/// <summary>
/// AI service abstraction - allows future provider swapping.
/// </summary>
public interface IAiService
{
    bool IsConfigured { get; }
    Task<string> GetDailyBriefingAsync(string userId);
    Task<string> GetNextActionRecommendationAsync(string userId);
    Task<List<string>> DecomposeGoalAsync(string goalDescription);
    Task<List<string>> DecomposeTaskAsync(string taskDescription);
    Task<string> ParseQuickCaptureAsync(string input);
    Task<string> ChatAsync(string userId, string message);
}

/// <summary>
/// Calendar provider abstraction for future integrations.
/// </summary>
public interface ICalendarProvider
{
    string ProviderName { get; }
    bool IsConfigured { get; }
    Task<bool> SyncAsync(string userId);
}
