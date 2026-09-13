using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AhmedOS.Web.Pages;

[Authorize]
public class InboxModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public InboxModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }
    public void OnGet() { }

    public async Task<IActionResult> OnPostQuickCaptureAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return RedirectToPage();
        var userId = _userManager.GetUserId(User)!;
        _db.TodoTasks.Add(new TodoTask
        {
            Title = title.Trim(),
            Status = TodoTaskStatus.Inbox,
            Priority = Priority.Medium,
            UserId = userId,
            Source = "quick-capture"
        });
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
