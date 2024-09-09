using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Domain.Constants.Logging;
using TaskManagement.Domain.Contracts.Logging;
using TaskManagement.Domain.Contracts.Task;
using TaskManagement.Persistence.Extensions;
using TaskManagement.Service.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TasksController : BaseController
    {
        private readonly ITaskManagerService _taskManagerService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskManagerService taskManagerService, ILogger<TasksController> logger)
        {
            _taskManagerService = taskManagerService;
            _logger = logger;

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            try
            {
                await _taskManagerService.CreateAsync(request.Title, request.Description, request.Status,
                request.Priority, request.DueDate, UserId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(TasksController))
                    .WithMethod(nameof(Create))
                    .WithComment(ex.ToString())
                    .WithParametres($"{nameof(request.Title)}: {request.Title} " +
                    $"{nameof(request.Description)}: {request.Description} " +
                    $"{nameof(request.Status)}: {request.Status} " +
                    $"{nameof(request.Priority)}: {request.Priority} " +
                    $"{nameof(request.DueDate)}: {request.DueDate}")
                    .WithOperation("POST /tasks")
                    .AsString());

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? sort,
            [FromQuery] IEnumerable<int>? status,
            [FromQuery] IEnumerable<int>? priority,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var tasks = await _taskManagerService.GetAllAsync(sort, status, priority, start, end, pageNumber, pageSize, UserId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(TasksController))
                    .WithMethod(nameof(GetAll))
                    .WithComment(ex.ToString())
                    .WithOperation("GET /tasks")
                    .WithParametres($"{nameof(sort)}: {sort} " +
                    $"{nameof(status)}: {string.Join(", ", status?.SelectMany(s => s.ToString()) ?? string.Empty)} " +
                    $"{nameof(priority)}: {string.Join(",", priority?.SelectMany(p => p.ToString()) ?? string.Empty)} " +
                    $"{nameof(start)}: {start} " +
                    $"{nameof(end)}: {end} " +
                    $"{nameof(pageNumber)}: {pageNumber} " +
                    $"{nameof(pageSize)}: {pageSize}")
                    .AsString());

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOne([FromRoute] Guid id)
        {
            try
            {
                var getOneResult = await _taskManagerService.GetOneAsync(id, UserId);
                return HandleRequest(getOneResult) ?? Ok(getOneResult.Payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(TasksController))
                    .WithMethod(nameof(GetOne))
                    .WithComment(ex.ToString())
                    .WithOperation($"GET /tasks/{id}")
                    .WithParametres($"{nameof(id)}: {id}")
                    .AsString());

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                var updateResult = await _taskManagerService.UpdateAsync(id, request.Title, request.Description, request.DueDate,
                request.Status, request.Priority, UserId);

                return HandleRequest(updateResult) ?? NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(TasksController))
                    .WithMethod(nameof(Update))
                    .WithComment(ex.ToString())
                    .WithOperation($"PUT /tasks/{id}")
                    .WithParametres($"{nameof(id)}: {id} " +
                    $"{nameof(request.Title)}: {request.Title} " +
                    $"{nameof(request.Description)}: {request.Description} " +
                    $"{nameof(request.DueDate)}: {request.DueDate} " +
                    $"{nameof(request.Status)}: {request.Status} " +
                    $"{nameof(request.Priority)}: {request.Priority}")
                    .AsString());

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var deleteResult = await _taskManagerService.DeleteAsync(id, UserId);
                return HandleRequest(deleteResult) ?? NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(TasksController))
                    .WithMethod(nameof(Delete))
                    .WithComment(ex.ToString())
                    .WithOperation($"DELETE /tasks/{id}")
                    .WithParametres($"{nameof(id)}: {id}")
                    .AsString());

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }
    }
}
