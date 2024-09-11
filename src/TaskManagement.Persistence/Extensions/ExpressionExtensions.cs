using System.Linq.Expressions;
using TaskManagement.Domain.Contracts.Task;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;
using TaskPriority = TaskManagement.Domain.Entities.TaskPriority;
using TaskEntity = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Persistence.Extensions
{
    /// <summary>
    /// Provides extension methods for building and applying expression filters on IQueryable collections.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines the current expression with a new filter using the logical OR operator.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter value.</typeparam>
        /// <param name="expression">The current expression.</param>
        /// <param name="parameter">The parameter expression representing the entity.</param>
        /// <param name="field">The field to filter on.</param>
        /// <param name="filter">The filter value.</param>
        /// <returns>A new expression representing the combined filter.</returns>
        public static Expression OrElseFilter<TFilter>(this Expression? expression, ParameterExpression parameter,
            string field, TFilter filter)
        {
            var binaryExpression = GetBinaryExpression(parameter, field, filter);

            return expression == null
                ? binaryExpression
                : Expression.OrElse(expression, binaryExpression);
        }

        /// <summary>
        /// Combines the current expression with a new filter using the logical AND operator.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter value.</typeparam>
        /// <param name="expression">The current expression.</param>
        /// <param name="parameter">The parameter expression representing the entity.</param>
        /// <param name="field">The field to filter on.</param>
        /// <param name="filter">The filter value.</param>
        /// <returns>A new expression representing the combined filter.</returns>
        public static Expression AndAlsoFilter<TFilter>(this Expression? expression, ParameterExpression parameter,
            string field, TFilter filter)
        {
            var binaryExpression = GetBinaryExpression(parameter, field, filter);

            return expression == null
                ? binaryExpression
                : Expression.AndAlso(expression, binaryExpression);
        }

        /// <summary>
        /// Applies a set of filters to the current expression based on the specified filter type.
        /// </summary>
        /// <param name="expression">The current expression.</param>
        /// <param name="parameter">The parameter expression representing the entity.</param>
        /// <param name="filterBy">The type of filter to apply.</param>
        /// <param name="filters">The collection of filter values.</param>
        /// <returns>A new expression representing the combined filters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported filter type is provided.</exception>
        public static Expression? ApplyFilter(this Expression? expression, ParameterExpression parameter,
            TaskFilterBy filterBy, IEnumerable<string> filters)
        {
            foreach (var filter in filters)
            {
                switch (filterBy)
                {
                    case TaskFilterBy.Status:
                        var statusFilter = Enum.Parse<TaskStatus>(filter);
                        expression = expression.OrElseFilter(parameter, nameof(TaskEntity.Status), statusFilter);
                        break;

                    case TaskFilterBy.Priority:
                        var priorityFilter = Enum.Parse<TaskPriority>(filter);
                        expression = expression.OrElseFilter(parameter, nameof(TaskEntity.Priority), priorityFilter);
                        break;

                    case TaskFilterBy.UserId:
                        var userIdFilter = Guid.Parse(filter);
                        expression = expression.AndAlsoFilter(parameter, nameof(TaskEntity.UserId), userIdFilter);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null);
                }
            }

            return expression;
        }

        /// <summary>
        /// Creates a binary expression for comparing a field with a filter value.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter value.</typeparam>
        /// <param name="parameter">The parameter expression representing the entity.</param>
        /// <param name="field">The field to filter on.</param>
        /// <param name="filter">The filter value.</param>
        /// <returns>A binary expression representing the comparison.</returns>
        private static BinaryExpression GetBinaryExpression<TFilter>(ParameterExpression parameter, string field, TFilter filter)
        {
            var memberAccess = Expression.PropertyOrField(parameter, field);
            return Expression.Equal(memberAccess, Expression.Constant(filter));
        }
    }
}
