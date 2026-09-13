using AhmedOS.Domain.Entities;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Pages;

[Authorize]
public class DEPIModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DEPIModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAddModuleAsync(
        string name,
        string? description,
        DateTime? startDate,
        DateTime? endDate,
        int? sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var module = new DEPIModule
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            StartDate = startDate,
            EndDate = endDate,
            SortOrder = sortOrder ?? 1,
            UserId = userId,
            Progress = 0,
            IsCompleted = false
        };

        _db.DEPIModules.Add(module);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddSessionAsync(
        int moduleId,
        string title,
        DateTime? sessionDate,
        int? durationMinutes,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(title)) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var module = await _db.DEPIModules.FirstOrDefaultAsync(m => m.Id == moduleId && m.UserId == userId);
        if (module == null) return RedirectToPage();

        var session = new DEPISession
        {
            DEPIModuleId = moduleId,
            Title = title.Trim(),
            SessionDate = sessionDate ?? DateTime.UtcNow,
            DurationMinutes = durationMinutes ?? 120,
            Notes = notes?.Trim(),
            IsAttended = false,
            UserId = userId
        };

        _db.DEPISessions.Add(session);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAttendanceAsync(int sessionId)
    {
        var userId = _userManager.GetUserId(User);
        var session = await _db.DEPISessions.Include(s => s.DEPIModule).FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
        if (session != null)
        {
            session.IsAttended = !session.IsAttended;
            await _db.SaveChangesAsync();

            // Recalculate module progress
            if (session.DEPIModule != null)
            {
                var sessions = await _db.DEPISessions.Where(s => s.DEPIModuleId == session.DEPIModuleId).ToListAsync();
                if (sessions.Count > 0)
                {
                    var attended = sessions.Count(s => s.IsAttended);
                    session.DEPIModule.Progress = (int)Math.Round((double)attended / sessions.Count * 100);
                    session.DEPIModule.IsCompleted = session.DEPIModule.Progress >= 100;
                    await _db.SaveChangesAsync();
                }
            }
        }
        return RedirectToPage();
    }
}
