using TaskManagement.Domain.Contracts.Common;
using TaskManagement.Domain.Contracts.Task;

namespace TaskManagement.Service.Interfaces
{
    /// <summary>
    /// Provides methods for managing tasks, including creating, retrieving, updating, and deleting tasks.
    /// </summary>
    public interface ITaskManagerService
    {
        /// <summary>
        /// Creates a new task with the specified details.
        /// </summary>
        /// <param name="title">The title of the task.</param>
        /// <param name="description">The description of the task. This parameter is optional.</param>
        /// <param name="status">The status of the task. This parameter is optional.</param>
        /// <param name="priority">The priority of the task. This parameter is optional.</param>
        /// <param name="dueDate">The due date of the task. This parameter is optional.</param>
        /// <param name="userId">The ID of the user associated with the task.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This parameter is optional.</param>
        /// <returns>A task representing the asynchronous operation, with a payload result containing the ID of the created task.</returns>
        Task<PayloadResult<Guid>> CreateAsync(string title, string? description, int? status,
            int? priority, DateTime? dueDate, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a task by its ID and user ID.
        /// </summary>
        /// <param name="id">The ID of the task.</param>
        /// <param name="userId">The ID of the user associated with the task.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a payload result containing the task DTO.</returns>
        Task<PayloadResult<TaskDto>> GetOneAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a collection of tasks based on the specified filtering, sorting, and pagination options.
        /// </summary>
        /// <param name="sort">The sorting option.</param>
        /// <param name="status">The status filter options.</param>
        /// <param name="priority">The priority filter options.</param>
        /// <param name="start">The start date for range filtering.</param>
        /// <param name="end">The end date for range filtering.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <param name="userId">The ID of the user associated with the tasks.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a read-only collection of task DTOs.</returns>
        Task<IReadOnlyCollection<TaskDto>> GetAllAsync(string? sort, IEnumerable<int>? status, IEnumerable<int>? priority,
            DateTime? start, DateTime? end, int pageNumber, int pageSize, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing task with the specified details.
        /// </summary>
        /// <param name="id">The ID of the task to update.</param>
        /// <param name="title">The new title of the task.</param>
        /// <param name="description">The new description of the task.</param>
        /// <param name="dueDate">The new due date of the task.</param>
        /// <param name="status">The new status of the task.</param>
        /// <param name="priority">The new priority of the task.</param>
        /// <param name="userId">The ID of the user associated with the task.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating the success or failure of the operation.</returns>
        Task<Result> UpdateAsync(Guid id, string? title, string? description, DateTime? dueDate, int? status,
            int? priority, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a task by its ID and user ID.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <param name="userId">The ID of the user associated with the task.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating the success or failure of the operation.</returns>
        Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}
