namespace TaskManagement.Domain.Contracts.Task
{
    public record TaskDto(
        string Id,
        string Title,
        string? Description,
        string? DueDate,
        string Status,
        string Priority,
        string CreatedAt,
        string UpdatedAt);
}
