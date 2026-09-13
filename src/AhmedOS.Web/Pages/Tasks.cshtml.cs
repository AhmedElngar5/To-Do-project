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
public class TasksModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPriorityEngine _priorityEngine;

    public TasksModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager, IPriorityEngine priorityEngine)
    {
        _db = db;
        _userManager = userManager;
        _priorityEngine = priorityEngine;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostCreateTaskAsync(
        string title, string? description, int priority, string? dueDate,
        int? categoryId, int? projectId, int? estimatedMinutes)
    {
        if (string.IsNullOrWhiteSpace(title))
            return RedirectToPage();

        var userId = _userManager.GetUserId(User)!;

        var task = new TodoTask
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            Priority = (Priority)priority,
            Status = dueDate != null ? TodoTaskStatus.Planned : TodoTaskStatus.Inbox,
            DueDate = !string.IsNullOrEmpty(dueDate) ? DateTime.Parse(dueDate) : null,
            CategoryId = categoryId > 0 ? categoryId : null,
            ProjectId = projectId > 0 ? projectId : null,
            EstimatedMinutes = estimatedMinutes > 0 ? estimatedMinutes : null,
            UserId = userId,
            Source = "manual"
        };

        var (score, reason) = _priorityEngine.CalculatePriorityScore(task);
        task.PriorityScore = score;
        task.PriorityReason = reason;

        _db.TodoTasks.Add(task);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMoveTaskAsync(int taskId, TodoTaskStatus newStatus, string? returnView)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _db.TodoTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task != null)
        {
            task.Status = newStatus;
            if (newStatus == TodoTaskStatus.Completed)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                task.CompletedAt = null;
            }

            var (score, reason) = _priorityEngine.CalculatePriorityScore(task);
            task.PriorityScore = score;
            task.PriorityReason = reason;

            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { view = returnView });
    }

    public async Task<IActionResult> OnPostRecalculatePrioritiesAsync(string? returnView)
    {
        var userId = _userManager.GetUserId(User)!;
        await _priorityEngine.RecalculateAllUserTaskPrioritiesAsync(userId);
        return RedirectToPage(new { view = returnView });
    }
}
