using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Contracts.Task;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Persistence.QueryObjects.Common;
using TaskManagement.Persistence.QueryObjects.Task;
using TaskEntity = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(TaskEntity task, CancellationToken cancellationToken = default)
        {
            await _dbContext.Tasks.AddAsync(task, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return task.Id;
        }

        public async Task DeleteAsync(TaskEntity task, CancellationToken cancellationToken = default)
        {
            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a filtered, sorted, and paginated list of tasks based on the provided options.
        /// </summary>
        /// <param name="options">The options for sorting, filtering, and pagination.</param>
        /// <returns>An <see cref="IQueryable{TaskEntity}"/> representing the filtered, sorted, and paginated tasks.</returns>
        public IQueryable<TaskEntity> GetAll(SortFilterTaskOptions options)
        {
            var query = _dbContext.Tasks.AsNoTracking();

            // Apply filter options (e.g., status, priority, user id)
            query = query.FilterTaskBy(options.FilterOptions);

            // Apply filter range options (e.g., due date)
            query = options.FilterRangeOptions.Aggregate(query, (current, filterOptions) =>
            {
                foreach (var filter in filterOptions.Value)
                {
                    var (f1, f2) = filter; //(string, string) filter
                    current = current.FilterTaskBy(filterOptions.Key, f1, f2); //key: enum filterByRange
                }

                return current;
            });

            query = query.OrderTasksBy(options.OrderOption);
            query = query.Paginate(options.PageNum, options.PageSize);
            return query;
        }
        public async Task<TaskEntity?> GetOneAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default)
        {
            _dbContext.Tasks.Update(task);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}