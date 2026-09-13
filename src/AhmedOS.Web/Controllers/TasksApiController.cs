using AhmedOS.Domain.Entities;
using AhmedOS.Domain.Enums;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AhmedOS.Web.Controllers;

[Route("api/tasks")]
[ApiController]
[Authorize]
public class TasksApiController : ControllerBase
{
    private readonly AhmedOSDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TasksApiController(AhmedOSDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteTask(int id)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _db.TodoTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null) return NotFound();

        task.Status = TodoTaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { success = true });
    }

    [HttpPost("{id}/uncomplete")]
    public async Task<IActionResult> UncompleteTask(int id)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _db.TodoTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null) return NotFound();

        task.Status = TodoTaskStatus.Planned;
        task.CompletedAt = null;
        await _db.SaveChangesAsync();

        return Ok(new { success = true });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _db.TodoTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null) return NotFound();

        task.IsDeleted = true; // Soft delete
        await _db.SaveChangesAsync();

        return Ok(new { success = true });
    }

    public record UpdateStatusRequest(TodoTaskStatus Status);
    public record QuickCreateRequest(string Title, TodoTaskStatus? Status, Priority? Priority);

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _db.TodoTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null) return NotFound();

        task.Status = request.Status;
        if (request.Status == TodoTaskStatus.Completed)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            task.CompletedAt = null;
        }
        await _db.SaveChangesAsync();

        return Ok(new { success = true, status = task.Status.ToString() });
    }

    [HttpPost("quick-create")]
    public async Task<IActionResult> QuickCreate([FromBody] QuickCreateRequest request)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request?.Title)) return BadRequest("Title is required");

        var task = new TodoTask
        {
            Title = request.Title.Trim(),
            Status = request.Status ?? TodoTaskStatus.Inbox,
            Priority = request.Priority ?? Priority.Medium,
            UserId = userId,
            Source = "quick-capture"
        };
        _db.TodoTasks.Add(task);
        await _db.SaveChangesAsync();

        return Ok(new { success = true, id = task.Id, title = task.Title });
    }
}
