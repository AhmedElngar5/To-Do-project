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
public class StudyModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudyModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAddTopicAsync(
        string name,
        string? description,
        string? icon,
        Difficulty difficulty,
        ConfidenceLevel confidence,
        StudyStatus status)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var topic = new StudyTopic
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Icon = string.IsNullOrWhiteSpace(icon) ? "📗" : icon.Trim(),
            Difficulty = difficulty,
            Confidence = confidence,
            Status = status,
            Progress = status == StudyStatus.Mastered ? 100 : (status == StudyStatus.Practicing ? 60 : (status == StudyStatus.Learning ? 25 : 0)),
            UserId = userId
        };

        _db.StudyTopics.Add(topic);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLogSessionAsync(
        int topicId,
        int durationMinutes,
        string? notes,
        ConfidenceLevel? confidenceAfter)
    {
        if (durationMinutes <= 0) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var topic = await _db.StudyTopics.FirstOrDefaultAsync(t => t.Id == topicId && t.UserId == userId);
        if (topic == null) return RedirectToPage();

        var session = new StudySession
        {
            StudyTopicId = topicId,
            DurationMinutes = durationMinutes,
            StartTime = DateTime.UtcNow.AddMinutes(-durationMinutes),
            EndTime = DateTime.UtcNow,
            Notes = notes?.Trim(),
            ConfidenceAfter = confidenceAfter,
            UserId = userId
        };

        _db.StudySessions.Add(session);

        topic.LastStudied = DateTime.UtcNow;
        topic.ReviewCount += 1;
        topic.NextReview = DateTime.UtcNow.AddDays(topic.Confidence == ConfidenceLevel.High ? 7 : (topic.Confidence == ConfidenceLevel.Medium ? 3 : 1));
        if (confidenceAfter.HasValue)
        {
            topic.Confidence = confidenceAfter.Value;
        }
        topic.Progress = Math.Min(100, topic.Progress + 5);

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
