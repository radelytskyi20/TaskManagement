using TaskManagement.Domain.Contracts.Auth;
using TaskManagement.Domain.Contracts.Common;

namespace TaskManagement.Service.Interfaces
{
    /// <summary>
    /// Interface for user management services.
    /// </summary>
    public interface IUserManagerService
    {
        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="username">The username of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="cancellationToken">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the registration.</returns>
        Task<Result> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs in a user asynchronously.
        /// </summary>
        /// <param name="identifier">The username or email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="cancellationToken">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PayloadResult{TokenResponse}"/> with the login token if successful.</returns>
        Task<PayloadResult<TokenResponse>> LoginAsync(string identifier, string password, CancellationToken cancellationToken = default);
    }

}
