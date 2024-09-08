namespace TaskManagement.Persistence.Interfaces
{
    /// <summary>
    /// Represents an interface for password hashing and verification of password hash.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes the given password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies if the given password matches the provided password hash.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="passwordHash">The password hash to compare against.</param>
        /// <returns>True if the password matches the password hash, otherwise false.</returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}
