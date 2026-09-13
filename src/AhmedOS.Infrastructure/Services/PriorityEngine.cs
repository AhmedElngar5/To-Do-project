using AhmedOS.Application.Interfaces;
using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Infrastructure.Services;

public class PriorityEngine : IPriorityEngine
{
    private readonly AhmedOSDbContext _db;

    public PriorityEngine(AhmedOSDbContext db)
    {
        _db = db;
    }

    public (double Score, string Reason) CalculatePriorityScore(TodoTask task)
    {
        double score = 0;
        var reasons = new List<string>();

        // 1. Base Priority Weight (max 40 pts)
        switch (task.Priority)
        {
            case Priority.Critical:
                score += 40;
                reasons.Add("Critical priority");
                break;
            case Priority.High:
                score += 30;
                reasons.Add("High priority");
                break;
            case Priority.Medium:
                score += 20;
                break;
            case Priority.Low:
                score += 10;
                break;
        }

        // 2. Due Date Urgency (max 35 pts)
        if (task.DueDate.HasValue)
        {
            var now = DateTime.UtcNow;
            var daysUntilDue = (task.DueDate.Value.Date - now.Date).TotalDays;

            if (daysUntilDue < 0)
            {
                score += 35;
                reasons.Add("Overdue");
            }
            else if (daysUntilDue == 0)
            {
                score += 30;
                reasons.Add("Due today");
            }
            else if (daysUntilDue <= 1)
            {
                score += 22;
                reasons.Add("Due tomorrow");
            }
            else if (daysUntilDue <= 3)
            {
                score += 15;
                reasons.Add("Due soon");
            }
            else if (daysUntilDue <= 7)
            {
                score += 8;
                reasons.Add("Due this week");
            }
        }

        // 3. Strategic Alignment (max 25 pts)
        if (task.GoalId.HasValue)
        {
            score += 12;
            reasons.Add("Linked to Goal");
        }

        if (task.ProjectId.HasValue)
        {
            score += 8;
            reasons.Add("Part of active Project");
        }

        if (task.CourseId.HasValue)
        {
            score += 10;
            reasons.Add("University course requirement");
        }

        if (task.DEPIModuleId.HasValue)
        {
            score += 8;
            reasons.Add("DEPI training milestone");
        }

        // 4. Status adjustments
        if (task.Status == TodoTaskStatus.InProgress)
        {
            score += 5;
            reasons.Add("In progress");
        }
        else if (task.Status == TodoTaskStatus.Blocked)
        {
            score = Math.Max(0, score - 20);
            reasons.Add("Blocked");
        }

        // Clamp between 0 and 100
        score = Math.Clamp(Math.Round(score, 1), 0, 100);

        var reason = reasons.Count > 0 ? string.Join(" • ", reasons) : "Standard task priority";

        return (score, reason);
    }

    public async Task RecalculateAllUserTaskPrioritiesAsync(string userId)
    {
        var activeTasks = await _db.TodoTasks
            .Where(t => t.UserId == userId && t.Status != TodoTaskStatus.Completed && t.Status != TodoTaskStatus.Cancelled && t.Status != TodoTaskStatus.Archived)
            .ToListAsync();

        foreach (var task in activeTasks)
        {
            var (score, reason) = CalculatePriorityScore(task);
            task.PriorityScore = score;
            task.PriorityReason = reason;
        }

        await _db.SaveChangesAsync();
    }
}
