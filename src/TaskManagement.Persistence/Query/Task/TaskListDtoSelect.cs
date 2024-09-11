using TaskManagement.Domain.Contracts.Task;
using TaskManagement.Persistence.Extensions;
using TaskEntity = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Persistence.QueryObjects.Task
{
    public static class TaskListDtoSelect
    {
        /// <summary>
        /// Maps the queryable collection of task entities to a queryable collection of task DTOs.
        /// </summary>
        /// <param name="tasks">The queryable collection of task entities to be mapped.</param>
        /// <returns>An <see cref="IQueryable{TaskDto}"/> representing the mapped task DTOs.</returns>
        public static IQueryable<TaskDto> MapTaskToDto(this IQueryable<TaskEntity> tasks) => tasks.Select(t => t.MapToDto());
    }
}
