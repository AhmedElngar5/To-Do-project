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
public class WaselModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPriorityEngine _priorityEngine;

    public WaselModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager, IPriorityEngine priorityEngine)
    {
        _db = db;
        _userManager = userManager;
        _priorityEngine = priorityEngine;
    }

    public Project? WaselProject { get; set; }
    public List<TodoTask> Tasks { get; set; } = [];
    public List<ProjectMilestone> Milestones { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        WaselProject = await _db.Projects
            .Include(p => p.Milestones)
            .FirstOrDefaultAsync(p => p.UserId == userId && (p.Name == "Wasel" || p.Type == ProjectType.Graduation));

        if (WaselProject == null)
        {
            var gradCat = await _db.Categories.FirstOrDefaultAsync(c => c.UserId == userId && c.Name == "Graduation Project");
            WaselProject = new Project
            {
                Name = "Wasel",
                Description = "AI-Based Child Family Reunification & Alternative Care Support System",
                Type = ProjectType.Graduation,
                Status = ProjectStatus.Active,
                Health = ProjectHealth.Healthy,
                Priority = Priority.Critical,
                Technologies = "C#, ASP.NET Core 10, SQL Server, Angular / Mobile, AI, NLP, Computer Vision",
                Color = "#f97316",
                Icon = "🔗",
                CategoryId = gradCat?.Id,
                UserId = userId,
                Progress = 35
            };
            _db.Projects.Add(WaselProject);
            await _db.SaveChangesAsync();

            // Add default milestones for graduation project
            var defaultMilestones = new List<ProjectMilestone>
            {
                new() { Title = "Project Proposal & Domain Research", DueDate = DateTime.UtcNow.AddDays(-30), IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-28), SortOrder = 1, ProjectId = WaselProject.Id, UserId = userId },
                new() { Title = "SRS & System Architecture Design", DueDate = DateTime.UtcNow.AddDays(-10), IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-5), SortOrder = 2, ProjectId = WaselProject.Id, UserId = userId },
                new() { Title = "Face Recognition & Biometric Matching Pipeline", DueDate = DateTime.UtcNow.AddDays(25), IsCompleted = false, SortOrder = 3, ProjectId = WaselProject.Id, UserId = userId },
                new() { Title = "Core Backend REST API (.NET 10 & SQL Server)", DueDate = DateTime.UtcNow.AddDays(45), IsCompleted = false, SortOrder = 4, ProjectId = WaselProject.Id, UserId = userId },
                new() { Title = "Web Portal & Mobile Application MVP", DueDate = DateTime.UtcNow.AddDays(70), IsCompleted = false, SortOrder = 5, ProjectId = WaselProject.Id, UserId = userId },
                new() { Title = "Final Defense, Testing & Field Documentation", DueDate = DateTime.UtcNow.AddDays(110), IsCompleted = false, SortOrder = 6, ProjectId = WaselProject.Id, UserId = userId },
            };
            _db.ProjectMilestones.AddRange(defaultMilestones);
            await _db.SaveChangesAsync();
        }

        Tasks = await _db.TodoTasks
            .Where(t => t.UserId == userId && t.ProjectId == WaselProject.Id)
            .OrderByDescending(t => t.PriorityScore ?? (double)t.Priority * 10)
            .ToListAsync();

        Milestones = await _db.ProjectMilestones
            .Where(m => m.UserId == userId && m.ProjectId == WaselProject.Id)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddTaskAsync(string title, int priority, string? dueDate, string? track)
    {
        if (string.IsNullOrWhiteSpace(title)) return RedirectToPage();
        var userId = _userManager.GetUserId(User)!;

        var project = await _db.Projects.FirstOrDefaultAsync(p => p.UserId == userId && (p.Name == "Wasel" || p.Type == ProjectType.Graduation));
        if (project == null) return RedirectToPage();

        var desc = !string.IsNullOrEmpty(track) ? $"[Track: {track}]" : null;

        var task = new TodoTask
        {
            Title = title.Trim(),
            Description = desc,
            ProjectId = project.Id,
            Priority = (Priority)priority,
            Status = TodoTaskStatus.Planned,
            DueDate = !string.IsNullOrEmpty(dueDate) ? DateTime.Parse(dueDate) : null,
            UserId = userId,
            Source = "wasel_workspace"
        };

        var (score, reason) = _priorityEngine.CalculatePriorityScore(task);
        task.PriorityScore = score;
        task.PriorityReason = reason;

        _db.TodoTasks.Add(task);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleMilestoneAsync(int milestoneId)
    {
        var userId = _userManager.GetUserId(User);
        var milestone = await _db.ProjectMilestones.FirstOrDefaultAsync(m => m.Id == milestoneId && m.UserId == userId);
        if (milestone != null)
        {
            milestone.IsCompleted = !milestone.IsCompleted;
            milestone.CompletedAt = milestone.IsCompleted ? DateTime.UtcNow : null;
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
