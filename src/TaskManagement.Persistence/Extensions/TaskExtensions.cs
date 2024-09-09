using TaskManagement.Domain.Contracts.Task;
using TaskEntity = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Persistence.Extensions
{
    public static class TaskExtensions
    {
        public static TaskDto MapToDto(this TaskEntity task)
        {
            return new TaskDto(
                Id: task.Id.ToString(),
                Title: task.Title,
                Description: task.Description,
                DueDate: task.DueDate.HasValue ? task.DueDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                Status: task.Status.ToString(),
                Priority: task.Priority.ToString(),
                CreatedAt: task.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                UpdatedAt: task.UpdatedAt.ToString("yyyy-MM-dd HH:mm")
            );
        }
    }
}
