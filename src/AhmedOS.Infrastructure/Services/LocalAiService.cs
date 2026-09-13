using AhmedOS.Application.Interfaces;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Infrastructure.Services;

/// <summary>
/// Smart local assistant service that works offline using heuristic engines and user context.
/// Ready to be extended with an external LLM API (OpenAI / Gemini / Anthropic).
/// </summary>
public class LocalAiService : IAiService
{
    private readonly AhmedOSDbContext _db;

    public LocalAiService(AhmedOSDbContext db)
    {
        _db = db;
    }

    public bool IsConfigured => true;

    public async Task<string> GetDailyBriefingAsync(string userId)
    {
        var today = DateTime.UtcNow.Date;
        var dueTodayCount = await _db.TodoTasks.CountAsync(t => t.UserId == userId && t.DueDate != null && t.DueDate.Value.Date == today && t.Status != TodoTaskStatus.Completed);
        var overdueCount = await _db.TodoTasks.CountAsync(t => t.UserId == userId && t.DueDate != null && t.DueDate.Value.Date < today && t.Status != TodoTaskStatus.Completed);
        var habitCount = await _db.Habits.CountAsync(h => h.UserId == userId && h.IsActive);

        return $"Good day, Ahmed! ☀️ Today you have {dueTodayCount} tasks due, {overdueCount} overdue items needing attention, and {habitCount} active habits to keep on streak. Focus on your top-priority milestone first.";
    }

    public async Task<string> GetNextActionRecommendationAsync(string userId)
    {
        var topTask = await _db.TodoTasks
            .Where(t => t.UserId == userId && t.Status != TodoTaskStatus.Completed && t.Status != TodoTaskStatus.Cancelled)
            .OrderByDescending(t => t.PriorityScore ?? (double)t.Priority * 10)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .FirstOrDefaultAsync();

        if (topTask == null)
            return "All caught up! 🎉 You have no pending tasks right now. Great time for deep learning or planning ahead.";

        var est = topTask.EstimatedMinutes.HasValue ? $" (~{topTask.EstimatedMinutes} min)" : "";
        var reason = !string.IsNullOrEmpty(topTask.PriorityReason) ? $" ({topTask.PriorityReason})" : "";
        return $"Recommended next action: **{topTask.Title}**{est}{reason}. Start a 25-minute Focus Session to get momentum!";
    }

    public Task<List<string>> DecomposeGoalAsync(string goalDescription)
    {
        var steps = new List<string>
        {
            $"Define clear requirements, deliverables & success metrics for '{goalDescription}'",
            "Research best practices, references, and existing architecture",
            "Break down core milestones and create task backlog",
            "Build initial prototype or MVP implementation",
            "Test, refactor, and review against success criteria",
            "Final polish, documentation, and celebrate completion!"
        };
        return Task.FromResult(steps);
    }

    public Task<List<string>> DecomposeTaskAsync(string taskDescription)
    {
        var subtasks = new List<string>
        {
            $"Step 1: Set up workspace and outline plan for '{taskDescription}'",
            "Step 2: Execute primary logic / initial draft",
            "Step 3: Review, verify and validate output",
            "Step 4: Final wrap up and mark as done"
        };
        return Task.FromResult(subtasks);
    }

    public Task<string> ParseQuickCaptureAsync(string input)
    {
        return Task.FromResult(input.Trim());
    }

    public async Task<string> ChatAsync(string userId, string message)
    {
        var msg = message.ToLowerInvariant().Trim();

        if (msg.Contains("what should i do") || msg.Contains("next") || msg.Contains("اعمل ايه") || msg.Contains("ابدأ بايه"))
        {
            return await GetNextActionRecommendationAsync(userId);
        }

        if (msg.Contains("briefing") || msg.Contains("summary") || msg.Contains("ملخص") || msg.Contains("صباح الخير"))
        {
            return await GetDailyBriefingAsync(userId);
        }

        if (msg.Contains("decompose") || msg.Contains("break down") || msg.Contains("قسم"))
        {
            return "To break down a goal or project into actionable steps, tell me: 'Decompose goal: [your goal name]' or check out the Goals page!";
        }

        if (msg.Contains("pomodoro") || msg.Contains("focus") || msg.Contains("تركيز"))
        {
            return "Tip: Use the **Focus** tab for a 25-minute Pomodoro sprint. Eliminate distractions, pick your single highest priority task, and work with undivided attention.";
        }

        if (msg.Contains("depi") || msg.Contains("training"))
        {
            var now = DateTime.UtcNow;
            var depiCount = await _db.DEPISessions.CountAsync(s => s.SessionDate >= now || !s.IsAttended);
            return $"You have {depiCount} pending or upcoming DEPI sessions. Head over to the **DEPI** tab to review attendance and study milestones.";
        }

        if (msg.Contains("exam") || msg.Contains("course") || msg.Contains("university") || msg.Contains("جامعة") || msg.Contains("كلية"))
        {
            var courses = await _db.Courses.Where(c => c.UserId == userId).CountAsync();
            return $"You are currently tracking {courses} university courses. Check the **University** page to monitor grades, assignments, and credit progress.";
        }

        return $"I'm your Ahmed OS Assistant 🤖! I can help you prioritize your day, suggest your next action, break down goals, or track your study progress. Try asking: 'What should I do now?' or 'Give me my daily briefing'.";
    }
}
