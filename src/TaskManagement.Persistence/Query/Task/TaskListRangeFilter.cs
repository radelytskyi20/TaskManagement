using TaskManagement.Domain.Contracts.Task;
using TaskEntity = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Persistence.QueryObjects.Task
{
    public static class TaskListRangeFilter
    {
        /// <summary>
        /// Filters the tasks based on the specified range filter options.
        /// </summary>
        /// <param name="tasks">The queryable collection of tasks to be filtered.</param>
        /// <param name="filterBy">The range filter option to apply.</param>
        /// <param name="start">The start date as a string for the range filter.</param>
        /// <param name="end">The end date as a string for the range filter.</param>
        /// <returns>An <see cref="IQueryable{TaskEntity}"/> representing the filtered tasks.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid range filter option is provided.</exception>
        public static IQueryable<TaskEntity> FilterTaskBy(
            this IQueryable<TaskEntity> tasks,
            TaskRangeFilterBy filterBy, string start, string end)
        {
            if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
                return tasks;

            switch (filterBy)
            {
                case TaskRangeFilterBy.NoFilter:
                    return tasks;

                case TaskRangeFilterBy.DueDate:
                    var startDate = DateTime.Parse(start);
                    var endDate = DateTime.Parse(end);
                    return tasks.Where(t => t.DueDate >= startDate && t.DueDate <= endDate);

                default:
                    throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null);
            }
        }
    }
}
