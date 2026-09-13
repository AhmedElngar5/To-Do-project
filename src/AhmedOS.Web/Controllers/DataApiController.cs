using System.Text;
using System.Text.Json;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Controllers;

[Route("api/data")]
[ApiController]
[Authorize]
public class DataApiController : ControllerBase
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DataApiController(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportAllData()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var tasks = await _db.TodoTasks.Where(t => t.UserId == userId).ToListAsync();
        var projects = await _db.Projects.Where(p => p.UserId == userId).ToListAsync();
        var goals = await _db.Goals.Where(g => g.UserId == userId).ToListAsync();
        var habits = await _db.Habits.Where(h => h.UserId == userId).ToListAsync();
        var notes = await _db.Notes.Where(n => n.UserId == userId).ToListAsync();
        var courses = await _db.Courses.Where(c => c.UserId == userId).ToListAsync();
        var depiModules = await _db.DEPIModules.Where(d => d.UserId == userId).ToListAsync();
        var studyTopics = await _db.StudyTopics.Where(s => s.UserId == userId).ToListAsync();
        var applications = await _db.JobApplications.Where(a => a.UserId == userId).ToListAsync();
        var settings = await _db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);

        var exportBundle = new
        {
            ExportedAt = DateTime.UtcNow,
            Platform = "Ahmed OS v1.0",
            User = User.Identity?.Name,
            Data = new
            {
                Tasks = tasks,
                Projects = projects,
                Goals = goals,
                Habits = habits,
                Notes = notes,
                Courses = courses,
                DEPIModules = depiModules,
                StudyTopics = studyTopics,
                JobApplications = applications,
                Settings = settings
            }
        };

        var json = JsonSerializer.Serialize(exportBundle, new JsonSerializerOptions { WriteIndented = true });
        var bytes = Encoding.UTF8.GetBytes(json);
        var filename = $"AhmedOS_Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";

        return File(bytes, "application/json", filename);
    }

    [HttpGet("export-tasks-csv")]
    public async Task<IActionResult> ExportTasksCsv()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var tasks = await _db.TodoTasks
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,Title,Status,Priority,DueDate,EstimatedMinutes,PriorityScore,PriorityReason,CreatedAt");

        foreach (var t in tasks)
        {
            var title = EscapeCsv(t.Title);
            var reason = EscapeCsv(t.PriorityReason ?? "");
            sb.AppendLine($"{t.Id},\"{title}\",{t.Status},{t.Priority},{t.DueDate:yyyy-MM-dd},{t.EstimatedMinutes},{t.PriorityScore},\"{reason}\",{t.CreatedAt:yyyy-MM-dd HH:mm}");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var filename = $"AhmedOS_Tasks_{DateTime.UtcNow:yyyyMMdd}.csv";

        return File(bytes, "text/csv", filename);
    }

    private static string EscapeCsv(string s)
    {
        return s.Replace("\"", "\"\"");
    }
}
