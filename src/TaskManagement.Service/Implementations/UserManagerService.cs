using TaskManagement.Domain.Constants;
using TaskManagement.Domain.Contracts.Auth;
using TaskManagement.Domain.Contracts.Common;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Persistence.Interfaces;
using TaskManagement.Service.Interfaces;
using User = TaskManagement.Domain.Entities.User;

namespace TaskManagement.Service.Implementations
{
    /// <summary>
    /// Implementation of the <see cref="IUserManagerService"/> interface for managing user operations.
    /// </summary>
    public class UserManagerService : IUserManagerService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IPasswordHasher _passwordHasher;

        public UserManagerService(IUserRepository userRepository, IJwtProvider jwtProvider,
            IPasswordValidator passwordValidator, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="username">The username of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="cancellationToken">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the registration.</returns>
        public async Task<Result> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default)
        {
            var userByUsername = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (userByUsername != null)
            {
                return Result.Failure(UserManagerServiceErrors.UsernameAlreadyExists);
            }

            var userByEmail = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (userByEmail != null)
            {
                return Result.Failure(UserManagerServiceErrors.EmailAlreadyExists);
            }

            var validationResult = _passwordValidator.Validate(password);
            if (!validationResult.Succeeded)
            {
                return Result.Failure(validationResult.Errors.ToArray());
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(password)
            };

            await _userRepository.AddAsync(user, cancellationToken);
            return Result.Success();
        }

        /// <summary>
        /// Logs in a user asynchronously.
        /// </summary>
        /// <param name="identifier">The username or email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="cancellationToken">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PayloadResult{TokenResponse}"/> with the login token if successful.</returns>
        public async Task<PayloadResult<TokenResponse>> LoginAsync(string identifier, string password,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByUsernameAsync(identifier, cancellationToken) ??
                       await _userRepository.GetByEmailAsync(identifier, cancellationToken);

            if (user == null) return PayloadResult<TokenResponse>.Failure(UserManagerServiceErrors.UserNotFound);

            var isValidPassword = _passwordHasher.VerifyPassword(password, user.PasswordHash);
            if (!isValidPassword) return PayloadResult<TokenResponse>.Failure(UserManagerServiceErrors.InvalidPassword);

            var token = _jwtProvider.GenerateJwtToken(user);
            return PayloadResult<TokenResponse>.Success(new TokenResponse(token));
        }
    }

}
