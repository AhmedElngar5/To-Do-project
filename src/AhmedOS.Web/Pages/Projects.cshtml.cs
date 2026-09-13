using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AhmedOS.Web.Pages;

[Authorize]
public class ProjectsModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectsModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostCreateProjectAsync(
        string name, string? description, int type, int priority,
        string? startDate, string? deadline, string? technologies)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToPage();
        var userId = _userManager.GetUserId(User)!;

        var project = new Project
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Type = (ProjectType)type,
            Priority = (Priority)priority,
            Status = ProjectStatus.Planning,
            StartDate = !string.IsNullOrEmpty(startDate) ? DateTime.Parse(startDate) : null,
            Deadline = !string.IsNullOrEmpty(deadline) ? DateTime.Parse(deadline) : null,
            Technologies = technologies?.Trim(),
            UserId = userId
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
