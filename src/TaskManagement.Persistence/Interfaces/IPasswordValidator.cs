using TaskManagement.Domain.Contracts.Common;

namespace TaskManagement.Persistence.Interfaces
{
    /// <summary>
    /// Represents an interface for password validation (complexity requirements).
    /// </summary>
    public interface IPasswordValidator
    {
        /// <summary>
        /// Validates the given password.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>The validation result.</returns>
        Result Validate(string password);
    }
}
