using AhmedOS.Application.Interfaces;
using AhmedOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AhmedOS.Web.Controllers;

[Route("api/ai")]
[ApiController]
[Authorize]
public class AiApiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly IPriorityEngine _priorityEngine;
    private readonly UserManager<ApplicationUser> _userManager;

    public AiApiController(IAiService aiService, IPriorityEngine priorityEngine, UserManager<ApplicationUser> userManager)
    {
        _aiService = aiService;
        _priorityEngine = priorityEngine;
        _userManager = userManager;
    }

    public record ChatRequest(string Message);
    public record DecomposeRequest(string Description);

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
            return BadRequest(new { error = "Message is required" });

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var reply = await _aiService.ChatAsync(userId, request.Message);
        return Ok(new { reply });
    }

    [HttpGet("briefing")]
    public async Task<IActionResult> GetBriefing()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var briefing = await _aiService.GetDailyBriefingAsync(userId);
        return Ok(new { briefing });
    }

    [HttpGet("next-action")]
    public async Task<IActionResult> GetNextAction()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var recommendation = await _aiService.GetNextActionRecommendationAsync(userId);
        return Ok(new { recommendation });
    }

    [HttpPost("decompose-goal")]
    public async Task<IActionResult> DecomposeGoal([FromBody] DecomposeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Description))
            return BadRequest(new { error = "Description is required" });

        var steps = await _aiService.DecomposeGoalAsync(request.Description);
        return Ok(new { steps });
    }

    [HttpPost("recalculate-priorities")]
    public async Task<IActionResult> RecalculatePriorities()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        await _priorityEngine.RecalculateAllUserTaskPrioritiesAsync(userId);
        return Ok(new { success = true, message = "Task priorities recalculated." });
    }
}
