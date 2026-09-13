using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Controllers;

[Route("api/notifications")]
[ApiController]
[Authorize]
public class NotificationApiController : ControllerBase
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationApiController(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = _userManager.GetUserId(User);
        var notifications = await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(20)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.Message,
                Type = n.Type.ToString(),
                n.IsRead,
                n.ActionUrl,
                TimeAgo = GetTimeAgo(n.CreatedAt)
            })
            .ToListAsync();

        if (notifications.Count == 0 && userId != null)
        {
            var welcome = new Notification
            {
                UserId = userId,
                Title = "Welcome to Ahmed OS 🚀",
                Message = "Your unified personal operating system is ready.",
                Type = NotificationType.System,
                IsRead = false,
                ActionUrl = "/Today",
                CreatedAt = DateTime.UtcNow
            };
            _db.Notifications.Add(welcome);
            await _db.SaveChangesAsync();

            return Ok(new[]
            {
                new
                {
                    welcome.Id,
                    welcome.Title,
                    welcome.Message,
                    Type = welcome.Type.ToString(),
                    welcome.IsRead,
                    welcome.ActionUrl,
                    TimeAgo = "Just now"
                }
            });
        }

        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = _userManager.GetUserId(User);
        var count = await _db.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);

        return Ok(new { count });
    }

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = _userManager.GetUserId(User);
        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notification == null) return NotFound();

        notification.IsRead = true;
        await _db.SaveChangesAsync();
        return Ok(new { success = true });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = _userManager.GetUserId(User);
        var unread = await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var n in unread)
        {
            n.IsRead = true;
        }

        await _db.SaveChangesAsync();
        return Ok(new { count = unread.Count });
    }

    private static string GetTimeAgo(DateTime dt)
    {
        var span = DateTime.UtcNow - dt;
        if (span.TotalMinutes < 1) return "Just now";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
        return $"{(int)span.TotalDays}d ago";
    }
}
