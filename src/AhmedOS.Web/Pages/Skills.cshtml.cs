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
public class SkillsModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public SkillsModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAddSkillAsync(
        string name,
        string? description,
        int? sortOrder,
        int? progress,
        ConfidenceLevel confidence,
        StudyStatus status)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToPage();
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        var skill = new Skill
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            SortOrder = sortOrder ?? 99,
            Progress = Math.Clamp(progress ?? 0, 0, 100),
            Confidence = confidence,
            Status = status,
            UserId = userId
        };

        _db.Skills.Add(skill);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateProgressAsync(int skillId, int progress, ConfidenceLevel confidence)
    {
        var userId = _userManager.GetUserId(User);
        var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == skillId && s.UserId == userId);
        if (skill != null)
        {
            skill.Progress = Math.Clamp(progress, 0, 100);
            skill.Confidence = confidence;
            skill.Status = skill.Progress >= 80 ? StudyStatus.Mastered : (skill.Progress >= 40 ? StudyStatus.Practicing : (skill.Progress > 0 ? StudyStatus.Learning : StudyStatus.NotStarted));
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
