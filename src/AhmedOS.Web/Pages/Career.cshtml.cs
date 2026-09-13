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
public class CareerModel : PageModel
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CareerModel(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAddApplicationAsync(
        string companyName,
        string position,
        JobApplicationStatus status,
        EmploymentType employmentType,
        string? location,
        string? salary,
        string? url,
        string? source,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(companyName) || string.IsNullOrWhiteSpace(position))
            return RedirectToPage();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToPage("/Account/Login");

        // Find or create company
        var company = await _db.Companies.FirstOrDefaultAsync(c => c.Name.ToLower() == companyName.Trim().ToLower() && c.UserId == userId);
        if (company == null)
        {
            company = new Company
            {
                Name = companyName.Trim(),
                UserId = userId,
                Location = location?.Trim()
            };
            _db.Companies.Add(company);
            await _db.SaveChangesAsync();
        }

        var app = new JobApplication
        {
            CompanyId = company.Id,
            Position = position.Trim(),
            Status = status,
            EmploymentType = employmentType,
            Location = location?.Trim(),
            Salary = salary?.Trim(),
            Url = url?.Trim(),
            Source = source?.Trim() ?? "LinkedIn",
            ApplicationDate = DateTime.UtcNow,
            Notes = notes?.Trim(),
            UserId = userId
        };

        _db.JobApplications.Add(app);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int applicationId, JobApplicationStatus status)
    {
        var userId = _userManager.GetUserId(User);
        var app = await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == userId);
        if (app != null)
        {
            app.Status = status;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
