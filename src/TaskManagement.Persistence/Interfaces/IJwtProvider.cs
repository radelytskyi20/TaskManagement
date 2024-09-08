using TaskManagement.Domain.Entities;

namespace TaskManagement.Persistence.Interfaces
{
    /// <summary>
    /// Represents an interface for generating JWT tokens.
    /// </summary>
    public interface IJwtProvider
    {
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token should be generated.</param>
        /// <returns>The generated JWT token.</returns>
        string GenerateJwtToken(User user);
    }
}
