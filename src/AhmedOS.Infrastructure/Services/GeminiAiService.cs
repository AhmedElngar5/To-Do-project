using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using AhmedOS.Application.Interfaces;
using AhmedOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AhmedOS.Infrastructure.Services;

/// <summary>
/// Cloud AI Service powered by Google Gemini API with intelligent local fallback.
/// </summary>
public class GeminiAiService : IAiService
{
    private readonly AhmedOSDbContext _db;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly LocalAiService _fallbackService;

    public GeminiAiService(AhmedOSDbContext db, IConfiguration config, HttpClient httpClient)
    {
        _db = db;
        _config = config;
        _httpClient = httpClient;
        _fallbackService = new LocalAiService(db);
    }

    public bool IsConfigured => true;

    private async Task<string?> GetApiKeyAsync(string? userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var settings = await _db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);
            if (!string.IsNullOrWhiteSpace(settings?.GeminiApiKey))
            {
                return settings.GeminiApiKey.Trim();
            }
        }

        var configKey = _config["Gemini:ApiKey"];
        if (!string.IsNullOrWhiteSpace(configKey) && configKey != "your_gemini_api_key_here")
        {
            return configKey.Trim();
        }

        return null;
    }

    public async Task<string> GetDailyBriefingAsync(string userId)
    {
        var apiKey = await GetApiKeyAsync(userId);
        if (string.IsNullOrEmpty(apiKey))
        {
            return await _fallbackService.GetDailyBriefingAsync(userId);
        }

        try
        {
            var today = DateTime.UtcNow.Date;
            var dueToday = await _db.TodoTasks.CountAsync(t => t.UserId == userId && t.DueDate != null && t.DueDate.Value.Date == today && !t.IsDeleted);
            var overdue = await _db.TodoTasks.CountAsync(t => t.UserId == userId && t.DueDate != null && t.DueDate.Value.Date < today && !t.IsDeleted && t.CompletedAt == null);
            var habits = await _db.Habits.CountAsync(h => h.UserId == userId && h.IsActive && !h.IsDeleted);

            var prompt = $"Act as the personal AI executive assistant for Ahmed OS. Write an energetic, inspiring, concise 2-sentence morning briefing for Ahmed. Context: {dueToday} tasks due today, {overdue} overdue, and {habits} active habits. Speak directly to Ahmed in professional, motivating tone.";
            var response = await CallGeminiAsync(apiKey, prompt);
            return !string.IsNullOrWhiteSpace(response) ? response : await _fallbackService.GetDailyBriefingAsync(userId);
        }
        catch
        {
            return await _fallbackService.GetDailyBriefingAsync(userId);
        }
    }

    public async Task<string> GetNextActionRecommendationAsync(string userId)
    {
        return await _fallbackService.GetNextActionRecommendationAsync(userId);
    }

    public async Task<List<string>> DecomposeGoalAsync(string goalDescription)
    {
        var apiKey = await GetApiKeyAsync(null);
        if (string.IsNullOrEmpty(apiKey))
        {
            return await _fallbackService.DecomposeGoalAsync(goalDescription);
        }

        try
        {
            var prompt = $"Break down this goal into 5 specific, high-impact sequential milestones/action items: '{goalDescription}'. Return ONLY the items separated by newlines, with no numbers or bullets.";
            var response = await CallGeminiAsync(apiKey, prompt);
            if (!string.IsNullOrWhiteSpace(response))
            {
                var lines = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(l => l.Trim().TrimStart('-', '*', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ' '))
                                    .Where(l => !string.IsNullOrWhiteSpace(l))
                                    .Take(6)
                                    .ToList();
                if (lines.Count > 0) return lines;
            }
        }
        catch { }

        return await _fallbackService.DecomposeGoalAsync(goalDescription);
    }

    public async Task<List<string>> DecomposeTaskAsync(string taskDescription)
    {
        var apiKey = await GetApiKeyAsync(null);
        if (string.IsNullOrEmpty(apiKey))
        {
            return await _fallbackService.DecomposeTaskAsync(taskDescription);
        }

        try
        {
            var prompt = $"Break down this engineering task into 3-4 subtasks: '{taskDescription}'. Return ONLY subtasks separated by newlines.";
            var response = await CallGeminiAsync(apiKey, prompt);
            if (!string.IsNullOrWhiteSpace(response))
            {
                var lines = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(l => l.Trim().TrimStart('-', '*', '0', '1', '2', '3', '4', '.', ' '))
                                    .Where(l => !string.IsNullOrWhiteSpace(l))
                                    .Take(4)
                                    .ToList();
                if (lines.Count > 0) return lines;
            }
        }
        catch { }

        return await _fallbackService.DecomposeTaskAsync(taskDescription);
    }

    public Task<string> ParseQuickCaptureAsync(string input)
    {
        return _fallbackService.ParseQuickCaptureAsync(input);
    }

    public async Task<string> ChatAsync(string userId, string message)
    {
        var apiKey = await GetApiKeyAsync(userId);
        if (string.IsNullOrEmpty(apiKey))
        {
            return await _fallbackService.ChatAsync(userId, message);
        }

        try
        {
            var systemContext = "You are Ahmed OS AI Copilot, a brilliant, concise, high-productivity executive partner for Ahmed Hany Kamal El Nagar. Ahmed is a software engineer studying Computer Science, participating in DEPI, building his graduation project Wasel, and learning .NET and AI. Be concise, direct, helpful, and motivating. Answer in the user's language (Arabic or English).";
            var prompt = $"{systemContext}\n\nUser: {message}\nCopilot:";
            var reply = await CallGeminiAsync(apiKey, prompt);
            return !string.IsNullOrWhiteSpace(reply) ? reply : await _fallbackService.ChatAsync(userId, message);
        }
        catch
        {
            return await _fallbackService.ChatAsync(userId, message);
        }
    }

    private async Task<string?> CallGeminiAsync(string apiKey, string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 600
            }
        };

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(12));
        var response = await _httpClient.PostAsJsonAsync(url, payload, cts.Token);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonNode.Parse(json);
        var text = doc?["candidates"]?[0]?["content"]?[parts()]?[0]?["text"]?.ToString()
                ?? doc?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

        return text?.Trim();

        static string parts() => "parts";
    }
}
