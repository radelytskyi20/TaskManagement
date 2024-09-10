using TaskEntity = TaskManagement.Domain.Entities.Task;
using TaskManagement.Domain.Contracts.Task;
using System.Linq.Expressions;
using TaskManagement.Persistence.Extensions;

namespace TaskManagement.Persistence.QueryObjects.Task
{
    public static class TaskListFilter
    {
        /// <summary>
        /// Filters a collection of tasks based on the specified filter options.
        /// </summary>
        /// <param name="tasks">The collection of tasks to filter.</param>
        /// <param name="filterOptions">A dictionary containing filter options and their corresponding values.</param>
        /// <returns>An <see cref="IQueryable{TaskEntity}"/> representing the filtered collection of tasks.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported filter option is provided.</exception>
        public static IQueryable<TaskEntity> FilterTaskBy(
            this IQueryable<TaskEntity> tasks,
            IDictionary<TaskFilterBy, ICollection<string>> filterOptions)
        {
            var parameter = Expression.Parameter(typeof(TaskEntity), "t");
            Expression? expression = null;

            // Order by filter option to ensure consistent query generation => for current implementation,
            // we need to filter tasks by UserId last
            foreach (var filterOption in filterOptions.OrderBy(fo => fo.Key))
            {
                Expression? appliedFilter = null;
                switch (filterOption.Key)
                {
                    case TaskFilterBy.NoFilter:
                        return tasks;

                    case TaskFilterBy.Status:
                        appliedFilter = appliedFilter.ApplyFilter(parameter, TaskFilterBy.Status, filterOption.Value);
                        break;

                    case TaskFilterBy.Priority:
                        appliedFilter = appliedFilter.ApplyFilter(parameter, TaskFilterBy.Priority, filterOption.Value);
                        break;

                    case TaskFilterBy.UserId:
                        appliedFilter = appliedFilter.ApplyFilter(parameter, TaskFilterBy.UserId, filterOption.Value);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(filterOption), filterOption, null);
                }

                // Combine filters
                // For example => (((t.Status == InProgress) OrElse (t.Status == Completed)) AND (t.UserId == C92645E8-1AF1-4BF6-8505-CC2B1AD74B23))
                expression = (expression != null && appliedFilter != null)
                    ? Expression.And(expression, appliedFilter)
                    : appliedFilter;
            }

            if (expression == null) return tasks;

            var lambda = Expression.Lambda<Func<TaskEntity, bool>>(expression, parameter);
            return tasks.Where(lambda);
        }
    }
}
