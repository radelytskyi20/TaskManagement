using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Domain.Constants.Task;
using TaskManagement.Domain.Contracts.Common;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the user ID from the authenticated user's claims.
        /// </summary>
        internal Guid UserId => User.Identity!.IsAuthenticated
            ? Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value)
            : Guid.Empty;

        /// <summary>
        /// Handles the request result and returns the appropriate <see cref="IActionResult"/>.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        /// <returns>An <see cref="IActionResult"/> representing the outcome of the request, or <c>null</c> if the request succeeded.</returns>
        internal IActionResult? HandleRequest(BaseResult result)
        {
            if (result.IsForbiden)
            {
                return Forbid();
            }
            if (!result.Succeeded && result.Errors.Contains(TaskManagerServiceErrors.TaskNotFound))
            {
                return NotFound();
            }
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return null; //result will be handled by the controller itself
        }
    }
}
