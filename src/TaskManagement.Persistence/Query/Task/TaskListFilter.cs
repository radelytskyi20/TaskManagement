using TaskEntity = TaskManagement.Domain.Entities.Task;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;
using TaskPriority = TaskManagement.Domain.Entities.TaskPriority;
using TaskManagement.Domain.Contracts.Task;

namespace TaskManagement.Persistence.QueryObjects.Task
{
    public static class TaskListFilter
    {
        /// <summary>
        /// Filters the tasks based on the specified filter options.
        /// </summary>
        /// <param name="tasks">The queryable collection of tasks to be filtered.</param>
        /// <param name="filterBy">The filter option to apply.</param>
        /// <param name="filter">The filter value as a string.</param>
        /// <returns>An <see cref="IQueryable{TaskEntity}"/> representing the filtered tasks.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid filter option is provided.</exception>
        public static IQueryable<TaskEntity> FilterTaskBy(
            this IQueryable<TaskEntity> tasks,
            TaskFilterBy filterBy, string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return tasks;

            switch (filterBy)
            {
                case TaskFilterBy.NoFilter:
                    return tasks;

                case TaskFilterBy.Status:
                    var statusFilter = Enum.Parse<TaskStatus>(filter);
                    return tasks.Where(t => t.Status == statusFilter);

                case TaskFilterBy.Priority:
                    var priorityFilter = Enum.Parse<TaskPriority>(filter);
                    return tasks.Where(t => t.Priority == priorityFilter);

                case TaskFilterBy.UserId:
                    var userIdFilter = Guid.Parse(filter);
                    return tasks.Where(t => t.UserId == userIdFilter);

                default:
                    throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null);
            }
        }
    }
}
