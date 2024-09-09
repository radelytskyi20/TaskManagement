using TaskManagement.Domain.Contracts.Task;
using TaskEntity = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Domain.Interfaces
{
    public interface ITaskRepository
    {
        Task CreateAsync(TaskEntity task, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TaskEntity?> GetOneAsync(Guid id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Retrieves a filtered, sorted, and paginated list of tasks based on the provided options.
        /// </summary>
        /// <param name="options">The options for sorting, filtering, and pagination.</param>
        /// <returns>An <see cref="IQueryable{TaskEntity}"/> representing the filtered, sorted, and paginated tasks.</returns>
        IQueryable<TaskEntity> GetAll(SortFilterTaskOptions options);

    }
}
