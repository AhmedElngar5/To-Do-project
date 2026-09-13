using AhmedOS.Domain.Entities;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Pages;

[Authorize]
public class UniversityModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public UniversityModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAddCourseAsync(
        string name,
        string code,
        string? instructor,
        string? semester,
        int? creditHours,
        string? color)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var course = new Course
        {
            Name = name.Trim(),
            Code = code?.Trim() ?? "CS",
            Instructor = instructor?.Trim(),
            Semester = semester?.Trim() ?? "Spring 2026",
            CreditHours = creditHours ?? 3,
            Color = color ?? "#3b82f6",
            UserId = userId,
            Progress = 0
        };

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddGradeAsync(
        int courseId,
        string name,
        string? type,
        double score,
        double maxScore,
        double? weight)
    {
        if (string.IsNullOrWhiteSpace(name) || maxScore <= 0) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var course = await _db.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.UserId == userId);
        if (course == null) return RedirectToPage();

        var grade = new CourseGrade
        {
            CourseId = courseId,
            Name = name.Trim(),
            Type = type?.Trim() ?? "Assignment",
            Score = score,
            MaxScore = maxScore,
            Weight = weight ?? 10,
            Date = DateTime.UtcNow,
            UserId = userId
        };

        _db.CourseGrades.Add(grade);
        await _db.SaveChangesAsync();

        // Update course progress based on average grade
        var allGrades = await _db.CourseGrades.Where(g => g.CourseId == courseId).ToListAsync();
        if (allGrades.Count > 0)
        {
            var validGrades = allGrades.Where(g => g.Score.HasValue && g.MaxScore.HasValue && g.MaxScore > 0).ToList();
            if (validGrades.Count > 0)
            {
                var avg = validGrades.Average(g => (g.Score!.Value / g.MaxScore!.Value) * 100);
                course.Progress = (int)Math.Clamp(avg, 0.0, 100.0);
                await _db.SaveChangesAsync();
            }
        }

        return RedirectToPage();
    }
}
