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
public class SettingsModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public SettingsModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostUpdateProfileAsync(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.FullName = fullName.Trim();
            await _userManager.UpdateAsync(user);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdatePreferencesAsync(
        string? workStartTime,
        string? workEndTime,
        int? maxDeepWorkHours,
        int? maxStudyHours,
        int? defaultTaskDurationMinutes,
        int? defaultBreakMinutes,
        string? theme,
        string? timezone,
        int? weekStartDay,
        TimeOfDay? preferredDeepWorkTime,
        TimeOfDay? preferredStudyTime,
        bool notificationsEnabled)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var settings = await _db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);
        if (settings == null)
        {
            settings = new UserSettings { UserId = userId };
            _db.UserSettings.Add(settings);
        }

        settings.WorkStartTime = workStartTime ?? "09:00";
        settings.WorkEndTime = workEndTime ?? "17:00";
        settings.MaxDeepWorkHours = maxDeepWorkHours ?? 4;
        settings.MaxStudyHours = maxStudyHours ?? 3;
        settings.DefaultTaskDurationMinutes = defaultTaskDurationMinutes ?? 30;
        settings.DefaultBreakMinutes = defaultBreakMinutes ?? 10;
        settings.Theme = theme ?? "dark";
        settings.Timezone = timezone ?? "Africa/Cairo";
        settings.WeekStartDay = weekStartDay ?? 0;
        settings.PreferredDeepWorkTime = preferredDeepWorkTime ?? TimeOfDay.Morning;
        settings.PreferredStudyTime = preferredStudyTime ?? TimeOfDay.Morning;
        settings.NotificationsEnabled = notificationsEnabled;

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
