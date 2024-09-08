using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        internal Guid UserId => User.Identity!.IsAuthenticated
            ? Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value)
            : Guid.Empty;
    }
}
