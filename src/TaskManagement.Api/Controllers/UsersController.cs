using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManagement.Domain.Constants.Auth;
using TaskManagement.Domain.Constants.Logging;
using TaskManagement.Domain.Constants.User;
using TaskManagement.Domain.Contracts.Auth;
using TaskManagement.Domain.Contracts.Logging;
using TaskManagement.Domain.Contracts.User;
using TaskManagement.Persistence.Extensions;
using TaskManagement.Service.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagerService _userManagerService;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserManagerService userManagerService, IOptions<JwtOptions> jwtOptions,
            ILogger<UsersController> logger)
        {
            _userManagerService = userManagerService;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        [HttpPost(UsersControllerRoutes.Register)]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var serviceResult = await _userManagerService.RegisterAsync(request.Username, request.Email, request.Password, cancellationToken);
                if (serviceResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created);
                }

                return BadRequest(serviceResult.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Registration))
                    .WithComment(ex.ToString())
                    .WithOperation(UsersControllerRoutes.Register)
                    .WithParametres($"{nameof(request.Username)}: {request.Username} {nameof(request.Email)}: {request.Email}")
                    .AsString());

                return StatusCode(StatusCodes.Status500InternalServerError, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(UsersControllerRoutes.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var serviceResult = await _userManagerService.LoginAsync(request.Identifier, request.Password, cancellationToken);
                if (serviceResult.Succeeded)
                {
                    HttpContext.Response.Cookies.Append(JwtConstants.AccessTokenCookie, serviceResult.Payload!.accessToken,
                        new CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpriesMinutes)
                        });
                    
                    return Ok(serviceResult.Payload);
                }

                return Unauthorized(serviceResult.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Login))
                    .WithComment(ex.ToString())
                    .WithOperation(UsersControllerRoutes.Login)
                    .WithParametres($"{nameof(request.Identifier)}: {request.Identifier}")
                    .AsString());

                return StatusCode(StatusCodes.Status500InternalServerError, LoggingConstants.InternalServerErrorMessage);
            }
        }
    }
}
