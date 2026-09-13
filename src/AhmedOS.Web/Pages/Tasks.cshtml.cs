using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AhmedOS.Web.Pages;

[Authorize]
public class TasksModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TasksModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
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

        _db.TodoTasks.Add(task);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }
}
