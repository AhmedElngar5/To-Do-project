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
public class CalendarModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CalendarModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostCreateEventAsync(
        string title, 
        CalendarEventType eventType, 
        DateTime startTime, 
        DateTime? endTime, 
        string? description, 
        string? location,
        int? courseId)
    {
        if (string.IsNullOrWhiteSpace(title)) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var calEvent = new CalendarEvent
        {
            Title = title.Trim(),
            EventType = eventType,
            StartTime = startTime,
            EndTime = endTime ?? startTime.AddHours(1),
            Description = description?.Trim(),
            Location = location?.Trim(),
            CourseId = courseId,
            UserId = userId
        };

        _db.CalendarEvents.Add(calEvent);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteEventAsync(int eventId)
    {
        var userId = _userManager.GetUserId(User);
        var calEvent = await _db.CalendarEvents.FirstOrDefaultAsync(e => e.Id == eventId && e.UserId == userId);
        if (calEvent != null)
        {
            _db.CalendarEvents.Remove(calEvent);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
