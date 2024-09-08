using TaskManagement.Persistence.Interfaces;

namespace TaskManagement.Persistence.Helpers
{
    /// <summary>
    /// Represents a class that is responsible for hashing and verifying passwords.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Hashes the given password using the BCrypt algorithm.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        public string HashPassword(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        /// <summary>
        /// Verifies if the given password matches the provided password hash.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="passwordHash">The password hash to compare against.</param>
        /// <returns>True if the password matches the password hash, otherwise false.</returns>
        public bool VerifyPassword(string password, string passwordHash) => BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
