using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Contracts.User
{
    public record RegistrationRequest(
    [Required] string Username,
    [Required][EmailAddress] string Email,
    [Required] string Password);
}
