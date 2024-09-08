using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Contracts.User
{
    public record LoginRequest(
        [Required] string Identifier,
        [Required] string Password);
}
