using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AhmedOS.Web.Pages;

[Authorize]
public class NotesModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public NotesModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }
    public void OnGet() { }

    public async Task<IActionResult> OnPostCreateNoteAsync(string title, string? content, int type, bool isPinned)
    {
        if (string.IsNullOrWhiteSpace(title)) return RedirectToPage();
        var userId = _userManager.GetUserId(User)!;
        _db.Notes.Add(new Note
        {
            Title = title.Trim(),
            Content = content?.Trim(),
            Type = (NoteType)type,
            IsPinned = isPinned,
            UserId = userId
        });
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
