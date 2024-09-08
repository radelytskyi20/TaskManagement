using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManagement.Domain.Constants;
using TaskManagement.Domain.Contracts.Auth;
using TaskManagement.Domain.Contracts.User;
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

        public UsersController(IUserManagerService userManagerService, IOptions<JwtOptions> jwtOptions)
        {
            _userManagerService = userManagerService;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost(UsersControllerRoutes.Register)]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var serviceResult = await _userManagerService.RegisterAsync(request.Username, request.Email, request.Password, cancellationToken);
                if (serviceResult.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(serviceResult.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                    HttpContext.Response.Cookies.Append(JwtConstants.ACCESS_TOKEN_COOKIE, serviceResult.Payload!.accessToken,
                        new CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpriesMinutes)
                        });
                    
                    return Ok(serviceResult.Payload);
                }

                return BadRequest(serviceResult.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
