using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AhmedOS.Web.Pages;

[Authorize]
public class GoalsModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public GoalsModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }
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
}
