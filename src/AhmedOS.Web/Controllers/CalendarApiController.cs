using System.Text;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Controllers;

[Route("api/calendar")]
[ApiController]
[Authorize]
public class CalendarApiController : ControllerBase
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CalendarApiController(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("export.ics")]
    public async Task<IActionResult> ExportIcs()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var events = await _db.CalendarEvents
            .Where(e => e.UserId == userId)
            .ToListAsync();

        var tasksWithDeadlines = await _db.TodoTasks
            .Where(t => t.UserId == userId && t.DueDate != null && t.Status != TodoTaskStatus.Completed && t.Status != TodoTaskStatus.Cancelled)
            .ToListAsync();

        var depiSessions = await _db.DEPISessions
            .Where(s => s.UserId == userId)
            .Include(s => s.DEPIModule)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//Ahmed OS//Personal Operating System//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");
        sb.AppendLine("X-WR-CALNAME:Ahmed OS — Schedule & Deadlines");
        sb.AppendLine("X-WR-TIMEZONE:Africa/Cairo");

        // 1. Calendar Events
        foreach (var ev in events)
        {
            var start = ev.StartTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
            var end = ev.EndTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
            var now = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:event-{ev.Id}-{ev.CreatedAt.Ticks}@ahmedos.local");
            sb.AppendLine($"DTSTAMP:{now}");
            sb.AppendLine($"DTSTART:{start}");
            sb.AppendLine($"DTEND:{end}");
            sb.AppendLine($"SUMMARY:{EscapeIcs(ev.Title)}");
            if (!string.IsNullOrEmpty(ev.Description))
            {
                sb.AppendLine($"DESCRIPTION:{EscapeIcs(ev.Description)}");
            }
            if (!string.IsNullOrEmpty(ev.Location))
            {
                sb.AppendLine($"LOCATION:{EscapeIcs(ev.Location)}");
            }
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("END:VEVENT");
        }

        // 2. Task Deadlines
        foreach (var t in tasksWithDeadlines)
        {
            var dueDate = t.DueDate!.Value.ToUniversalTime();
            var start = dueDate.ToString("yyyyMMddTHHmmssZ");
            var end = dueDate.AddMinutes(t.EstimatedMinutes ?? 60).ToString("yyyyMMddTHHmmssZ");
            var now = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:task-{t.Id}-{t.CreatedAt.Ticks}@ahmedos.local");
            sb.AppendLine($"DTSTAMP:{now}");
            sb.AppendLine($"DTSTART:{start}");
            sb.AppendLine($"DTEND:{end}");
            sb.AppendLine($"SUMMARY:[Task] {EscapeIcs(t.Title)}");
            sb.AppendLine($"DESCRIPTION:Priority: {t.Priority}{(string.IsNullOrEmpty(t.Description) ? "" : " | " + EscapeIcs(t.Description))}");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("END:VEVENT");
        }

        // 3. DEPI Sessions
        foreach (var ds in depiSessions)
        {
            var sessionDate = (ds.SessionDate ?? DateTime.UtcNow).ToUniversalTime();
            var duration = ds.DurationMinutes ?? 120;
            var start = sessionDate.ToString("yyyyMMddTHHmmssZ");
            var end = sessionDate.AddMinutes(duration).ToString("yyyyMMddTHHmmssZ");
            var now = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:depi-session-{ds.Id}@ahmedos.local");
            sb.AppendLine($"DTSTAMP:{now}");
            sb.AppendLine($"DTSTART:{start}");
            sb.AppendLine($"DTEND:{end}");
            sb.AppendLine($"SUMMARY:[DEPI] {EscapeIcs(string.IsNullOrEmpty(ds.Title) ? (ds.DEPIModule?.Name ?? "DEPI Session") : ds.Title)}");
            sb.AppendLine($"DESCRIPTION:DEPI Module: {EscapeIcs(ds.DEPIModule?.Name ?? "")}{(ds.IsAttended ? " (Attended)" : " (Upcoming)")}");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("END:VEVENT");
        }

        sb.AppendLine("END:VCALENDAR");

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/calendar", $"AhmedOS_Schedule_{DateTime.UtcNow:yyyyMMdd}.ics");
    }

    private static string EscapeIcs(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Replace("\\", "\\\\")
                   .Replace(";", "\\;")
                   .Replace(",", "\\,")
                   .Replace("\r\n", "\\n")
                   .Replace("\n", "\\n");
    }
}
