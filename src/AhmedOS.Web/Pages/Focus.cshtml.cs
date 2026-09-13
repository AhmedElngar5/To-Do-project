using AhmedOS.Domain.Entities;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AhmedOS.Web.Pages;

[Authorize]
public class FocusModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager) : PageModel
{
    public void OnGet() { }

    public async Task<IActionResult> OnPostLogSessionAsync(int plannedMinutes, int actualMinutes, int? taskId)
    {
        var userId = userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var session = new FocusSession
        {
            UserId = userId,
            StartTime = DateTime.UtcNow.AddMinutes(-actualMinutes),
            EndTime = DateTime.UtcNow,
            PlannedMinutes = plannedMinutes > 0 ? plannedMinutes : 25,
            ActualMinutes = actualMinutes > 0 ? actualMinutes : 25,
            IsCompleted = true,
            TaskId = taskId,
            CreatedAt = DateTime.UtcNow
        };

        db.FocusSessions.Add(session);
        await db.SaveChangesAsync();

        return new JsonResult(new { success = true, actualMinutes = session.ActualMinutes });
    }
}
