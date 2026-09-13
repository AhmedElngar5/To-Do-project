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

    public async Task<IActionResult> OnPostAddFlashcardAsync(int topicId, string front, string back)
    {
        if (string.IsNullOrWhiteSpace(front) || string.IsNullOrWhiteSpace(back)) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var card = new Flashcard
        {
            StudyTopicId = topicId,
            Front = front.Trim(),
            Back = back.Trim(),
            Box = 1,
            NextReviewDate = DateTime.UtcNow,
            UserId = userId
        };

        _db.Flashcards.Add(card);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReviewFlashcardAsync(int cardId, int rating)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var card = await _db.Flashcards.FirstOrDefaultAsync(c => c.Id == cardId && c.UserId == userId);
        if (card != null)
        {
            card.LastReviewedAt = DateTime.UtcNow;
            card.ReviewCount += 1;

            if (rating == 3) // Easy
            {
                card.Box = Math.Min(5, card.Box + 1);
            }
            else if (rating == 1) // Hard
            {
                card.Box = Math.Max(1, card.Box - 1);
            }

            var daysToAdd = card.Box switch
            {
                1 => 1,
                2 => 3,
                3 => 7,
                4 => 14,
                _ => 30
            };

            card.NextReviewDate = DateTime.UtcNow.AddDays(daysToAdd);
            await _db.SaveChangesAsync();
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
        {
            return new JsonResult(new { success = true, nextReview = card?.NextReviewDate?.ToString("yyyy-MM-dd"), box = card?.Box });
        }

        return RedirectToPage();
    }
}
