using AhmedOS.Domain.Entities;

namespace AhmedOS.Application.Interfaces;

public interface IPriorityEngine
{
    (double Score, string Reason) CalculatePriorityScore(TodoTask task);
    Task RecalculateAllUserTaskPrioritiesAsync(string userId);
}
