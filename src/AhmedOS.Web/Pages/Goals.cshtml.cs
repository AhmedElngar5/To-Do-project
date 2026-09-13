using AhmedOS.Application.Interfaces;
using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Pages;

[Authorize]
public class GoalsModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAiService _aiService;
    private readonly IPriorityEngine _priorityEngine;

    public GoalsModel(
        AhmedOSDbContext db,
        UserManager<ApplicationUser> userManager,
        IAiService aiService,
        IPriorityEngine priorityEngine)
    {
        _db = db;
        _userManager = userManager;
        _aiService = aiService;
        _priorityEngine = priorityEngine;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostCreateGoalAsync(string title, string? whyItMatters, int level, string? deadline)
    {
        if (string.IsNullOrWhiteSpace(title)) return RedirectToPage();
        var userId = _userManager.GetUserId(User)!;
        _db.Goals.Add(new Goal
        {
            Title = title.Trim(),
            WhyItMatters = whyItMatters?.Trim(),
            Level = (GoalLevel)level,
            Deadline = !string.IsNullOrEmpty(deadline) ? DateTime.Parse(deadline) : null,
            UserId = userId
        });
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDecomposeGoalAsync(int goalId)
    {
        var userId = _userManager.GetUserId(User)!;
        var goal = await _db.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
        if (goal == null) return RedirectToPage();

        var steps = await _aiService.DecomposeGoalAsync(goal.Title);
        foreach (var step in steps)
        {
            var task = new TodoTask
            {
                Title = step,
                GoalId = goal.Id,
                UserId = userId,
                Priority = Priority.High,
                Status = TodoTaskStatus.Planned,
                Source = "ai_decomposition"
            };
            var (score, reason) = _priorityEngine.CalculatePriorityScore(task);
            task.PriorityScore = score;
            task.PriorityReason = reason;

            _db.TodoTasks.Add(task);
        }

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
