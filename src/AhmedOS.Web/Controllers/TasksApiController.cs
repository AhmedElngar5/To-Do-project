using System.Text.Json;
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

    public class UpdateStatusRequest
    {
        public JsonElement Status { get; set; }
    }

    public class QuickCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public JsonElement? Status { get; set; }
        public JsonElement? Priority { get; set; }
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _db.TodoTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null) return NotFound();

        TodoTaskStatus newStatus;
        if (request.Status.ValueKind == JsonValueKind.Number && request.Status.TryGetInt32(out var num))
        {
            newStatus = (TodoTaskStatus)num;
        }
        else if (request.Status.ValueKind == JsonValueKind.String)
        {
            var str = request.Status.GetString() ?? "";
            if (int.TryParse(str, out var parsedNum))
            {
                newStatus = (TodoTaskStatus)parsedNum;
            }
            else if (Enum.TryParse<TodoTaskStatus>(str, true, out var parsedEnum))
            {
                newStatus = parsedEnum;
            }
            else
            {
                return BadRequest("Invalid status string.");
            }
        }
        else
        {
            return BadRequest("Invalid status format.");
        }

        task.Status = newStatus;
        if (newStatus == TodoTaskStatus.Completed)
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

        TodoTaskStatus status = TodoTaskStatus.Inbox;
        if (request.Status.HasValue)
        {
            var elem = request.Status.Value;
            if (elem.ValueKind == JsonValueKind.Number && elem.TryGetInt32(out var num))
                status = (TodoTaskStatus)num;
            else if (elem.ValueKind == JsonValueKind.String && Enum.TryParse<TodoTaskStatus>(elem.GetString(), true, out var parsed))
                status = parsed;
        }

        Priority priority = Priority.Medium;
        if (request.Priority.HasValue)
        {
            var elem = request.Priority.Value;
            if (elem.ValueKind == JsonValueKind.Number && elem.TryGetInt32(out var num))
                priority = (Priority)num;
            else if (elem.ValueKind == JsonValueKind.String && Enum.TryParse<Priority>(elem.GetString(), true, out var parsed))
                priority = parsed;
        }

        var task = new TodoTask
        {
            Title = request.Title.Trim(),
            Status = status,
            Priority = priority,
            UserId = userId,
            Source = "quick-capture"
        };
        _db.TodoTasks.Add(task);
        await _db.SaveChangesAsync();

        return Ok(new { success = true, id = task.Id, title = task.Title });
    }
}
