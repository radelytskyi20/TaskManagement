using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Domain.Constants.Logging;
using TaskManagement.Domain.Contracts.Logging;
using TaskManagement.Persistence.Extensions;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Check()
        {
            try
            {
                return Ok("Api is online!");
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(HealthController))
                    .WithMethod(nameof(Check))
                    .WithComment(ex.ToString())
                    .WithParametres(LoggingConstants.NoParameters)
                    .WithOperation("GET /health")
                    .AsString());

                return StatusCode(StatusCodes.Status500InternalServerError, LoggingConstants.InternalServerErrorMessage);
            }
        }
    }
}
