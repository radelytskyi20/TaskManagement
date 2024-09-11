using TaskManagement.Domain.Contracts.Task;
using TaskEntity = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Persistence.QueryObjects.Task
{
    public static class TaskListSort
    {
        /// <summary>
        /// Orders the tasks based on the specified ordering options.
        /// </summary>
        /// <param name="tasks">The queryable collection of tasks to be ordered.</param>
        /// <param name="orderByOptions">The ordering options to apply.</param>
        /// <returns>An <see cref="IQueryable{TaskEntity}"/> representing the ordered tasks.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid ordering option is provided.</exception>
        public static IQueryable<TaskEntity> OrderTasksBy(
            this IQueryable<TaskEntity> tasks,
            TaskOrderBy orderByOptions)
        {
            switch (orderByOptions)
            {
                case TaskOrderBy.SimpleOrder:
                    return tasks.OrderByDescending(t => t.CreatedAt); // because of paging we always need to sort => show latest entries first

                case TaskOrderBy.DueDate:
                    return tasks.OrderBy(t => t.DueDate);

                case TaskOrderBy.DueDateDesc:
                    return tasks.OrderByDescending(t => t.DueDate);

                case TaskOrderBy.Priority:
                    return tasks.OrderBy(t => t.Priority);

                case TaskOrderBy.PriorityDesc:
                    return tasks.OrderByDescending(t => t.Priority);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(orderByOptions), orderByOptions, null);
            }
        }
    }
}
