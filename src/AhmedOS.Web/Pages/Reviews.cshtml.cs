using AhmedOS.Domain.Entities;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Pages;

[Authorize]
public class ReviewsModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostSaveReviewAsync(
        string? accomplishments,
        string? challenges,
        string? tomorrowPlan,
        string? notes,
        int? moodRating,
        int? productivityRating)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var today = DateTime.UtcNow.Date;
        var review = await _db.DailyReviews.FirstOrDefaultAsync(r => r.UserId == userId && r.Date.Date == today);

        if (review == null)
        {
            review = new DailyReview
            {
                Date = today,
                UserId = userId
            };
            _db.DailyReviews.Add(review);
        }

        review.Accomplishments = accomplishments?.Trim();
        review.Challenges = challenges?.Trim();
        review.TomorrowPlan = tomorrowPlan?.Trim();
        review.Notes = notes?.Trim();
        review.MoodRating = moodRating;
        review.ProductivityRating = productivityRating;

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
