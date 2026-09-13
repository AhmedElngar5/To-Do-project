using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Controllers;

[Route("api/search")]
[ApiController]
[Authorize]
public class SearchApiController : ControllerBase
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public SearchApiController(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(new { results = Array.Empty<object>() });

        var userId = _userManager.GetUserId(User);
        var query = q.ToLower();
        var results = new List<object>();

        // Search tasks
        var tasks = await _db.TodoTasks
            .Where(t => t.UserId == userId && (t.Title.ToLower().Contains(query) || (t.Description != null && t.Description.ToLower().Contains(query))))
            .Take(5)
            .Select(t => new { t.Id, t.Title, Type = "Task", Icon = "✅", Url = "/Tasks", Sub = t.Status.ToString() })
            .ToListAsync();
        results.AddRange(tasks);

        // Search projects
        var projects = await _db.Projects
            .Where(p => p.UserId == userId && (p.Name.ToLower().Contains(query) || (p.Description != null && p.Description.ToLower().Contains(query))))
            .Take(3)
            .Select(p => new { p.Id, Title = p.Name, Type = "Project", Icon = "🚀", Url = "/Projects", Sub = p.Status.ToString() })
            .ToListAsync();
        results.AddRange(projects);

        // Search goals
        var goals = await _db.Goals
            .Where(g => g.UserId == userId && (g.Title.ToLower().Contains(query) || (g.Description != null && g.Description.ToLower().Contains(query))))
            .Take(3)
            .Select(g => new { g.Id, g.Title, Type = "Goal", Icon = "🎯", Url = "/Goals", Sub = g.Level.ToString() })
            .ToListAsync();
        results.AddRange(goals);

        // Search notes
        var notes = await _db.Notes
            .Where(n => n.UserId == userId && (n.Title.ToLower().Contains(query) || (n.Content != null && n.Content.ToLower().Contains(query))))
            .Take(3)
            .Select(n => new { n.Id, n.Title, Type = "Note", Icon = "📝", Url = "/Notes", Sub = n.Type.ToString() })
            .ToListAsync();
        results.AddRange(notes);

        // Search study topics
        var topics = await _db.StudyTopics
            .Where(s => s.UserId == userId && s.Name.ToLower().Contains(query))
            .Take(3)
            .Select(s => new { s.Id, Title = s.Name, Type = "Study", Icon = "📚", Url = "/Study", Sub = s.Status.ToString() })
            .ToListAsync();
        results.AddRange(topics);

        // Search habits
        var habits = await _db.Habits
            .Where(h => h.UserId == userId && h.Name.ToLower().Contains(query))
            .Take(3)
            .Select(h => new { h.Id, Title = h.Name, Type = "Habit", Icon = "🔄", Url = "/Habits", Sub = $"🔥 {h.CurrentStreak}" })
            .ToListAsync();
        results.AddRange(habits);

        return Ok(new { results });
    }
}
