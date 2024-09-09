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

        public async Task CreateAsync(TaskEntity task, CancellationToken cancellationToken = default)
        {
            await _dbContext.Tasks.AddAsync(task, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var task = await _dbContext.Tasks.FindAsync(id, cancellationToken);
            if (task is null) return;

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
            var query = _dbContext.Tasks.Include(task => task.User).AsNoTracking();

            // Apply filter options (e.g., status, priority, user id)
            query = options.FilterOptions.Aggregate(query, (current, filterOptions) =>
            {
                foreach (var filter in filterOptions.Value)
                    current = current.FilterTaskBy(filterOptions.Key, filter); //key: enum filterBy, value: string filter

                return current;
            });

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

            return query
                .OrderTasksBy(options.OrderOption)
                .Paginate(options.PageNum, options.PageSize);
        }
        public async Task<TaskEntity?> GetOneAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .AsNoTracking()
                .Include(task => task.User)
                .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default)
        {
            _dbContext.Tasks.Update(task);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}