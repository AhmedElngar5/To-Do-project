using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AhmedOS.Web.Pages;

[Authorize]
public class HabitsModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public HabitsModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }
    public void OnGet() { }

    public async Task<IActionResult> OnPostCreateHabitAsync(string name, string? icon, string? color)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToPage();
        var userId = _userManager.GetUserId(User)!;
        _db.Habits.Add(new Habit
        {
            Name = name.Trim(),
            Icon = icon ?? "📋",
            Color = color ?? "#6366f1",
            Schedule = RecurrenceType.Daily,
            TargetCount = 1,
            TargetUnit = "session",
            UserId = userId
        });
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
