using TaskManagement.Domain.Contracts.Common;
using TaskManagement.Domain.Contracts.Task;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Service.Interfaces;
using TaskEntity = TaskManagement.Domain.Entities.Task;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;
using TaskPriority = TaskManagement.Domain.Entities.TaskPriority;
using TaskManagement.Persistence.QueryObjects.Task;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Persistence.Extensions;
using TaskManagement.Domain.Constants;

namespace TaskManagement.Service.Implementations
{
    /// <summary>
    /// Provides implementation for managing tasks, including creating, retrieving, updating, and deleting tasks.
    /// </summary>
    public class TaskManagerService : ITaskManagerService
    {
        private readonly ITaskRepository _taskRepository;
        public TaskManagerService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        /// <inheritdoc />
        public async Task CreateAsync(string title, string? description, int? status, int? priority, DateTime? dueDate,
            Guid userId, CancellationToken cancellationToken = default)
        {
            var task = new TaskEntity
            {
                Title = title,
                Description = description,
                Status = status.HasValue ? (TaskStatus)status.Value : TaskStatus.Pending,
                Priority = priority.HasValue ? (TaskPriority)priority.Value : TaskPriority.Low,
                DueDate = dueDate,
                UserId = userId
            };

            await _taskRepository.CreateAsync(task, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var taskToDelete = await _taskRepository.GetOneAsync(id, cancellationToken);
            if (taskToDelete is null)
            {
                return Result.Failure(TaskManagerServiceErrors.TaskNotFound);
            }

            if (!IsAuthorized(taskToDelete, userId))
            {
                return Result.Forbid();
            }

            await _taskRepository.DeleteAsync(id, cancellationToken);
            return Result.Success();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<TaskDto>> GetAllAsync(string? sort, IEnumerable<int>? status,
            IEnumerable<int>? priority, DateTime? start, DateTime? end, int pageNumber, int pageSize, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var sortFilterOptions = new SortFilterTaskOptions(sort, status, priority, start, end, userId, pageNumber, pageSize);
            return await _taskRepository.GetAll(sortFilterOptions)
                .MapTaskToDto()
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PayloadResult<TaskDto>> GetOneAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetOneAsync(id, cancellationToken);
            if (task is null)
            {
                return PayloadResult<TaskDto>.Failure(TaskManagerServiceErrors.TaskNotFound);
            }

            if (!IsAuthorized(task, userId))
            {
                return PayloadResult<TaskDto>.Forbid();
            }

            return PayloadResult<TaskDto>.Success(task.MapToDto());
        }

        /// <inheritdoc />
        public async Task<Result> UpdateAsync(Guid id, string? title, string? description, DateTime? dueDate, int? status, int? priority,
            Guid userId, CancellationToken cancellationToken = default)
        {
            var taskToUpdate = await _taskRepository.GetOneAsync(id, cancellationToken);
            if (taskToUpdate is null)
            {
                return Result.Failure(TaskManagerServiceErrors.TaskNotFound);
            }

            if (!IsAuthorized(taskToUpdate, userId))
            {
                return Result.Forbid();
            }

            taskToUpdate.Title = title ?? taskToUpdate.Title;
            taskToUpdate.Description = description ?? taskToUpdate.Description;
            taskToUpdate.DueDate = dueDate ?? taskToUpdate.DueDate;
            taskToUpdate.Status = status.HasValue ? (TaskStatus)status.Value : taskToUpdate.Status;
            taskToUpdate.Priority = priority.HasValue ? (TaskPriority)priority.Value : taskToUpdate.Priority;

            await _taskRepository.UpdateAsync(taskToUpdate, cancellationToken);
            return Result.Success();
        }

        /// <summary>
        /// Checks if the user is authorized to perform actions on the task.
        /// </summary>
        /// <param name="task">The task entity.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns><c>true</c> if the user is authorized; otherwise, <c>false</c>.</returns>
        private static bool IsAuthorized(TaskEntity task, Guid userId) => task.UserId == userId;
    }
}
