using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Contracts.Task
{
    public record UpdateTaskRequest(
        string? Title,
        string? Description,
        DateTime? DueDate,
        [Range(0, 2)] int? Status,
        [Range(0, 2)] int? Priority);
}
