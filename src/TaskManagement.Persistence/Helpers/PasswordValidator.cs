using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using TaskManagement.Domain.Constants;
using TaskManagement.Domain.Contracts.Auth;
using TaskManagement.Domain.Contracts.Common;
using TaskManagement.Persistence.Interfaces;

namespace TaskManagement.Persistence.Helpers
{
    /// <summary>
    /// Represents a password validator (complexity requirements) that implements the <see cref="IPasswordValidator"/> interface.
    /// </summary>
    public class PasswordValidator : IPasswordValidator
    {
        private readonly PasswordComplexityOptions _passwordOptions;

        public PasswordValidator(IOptions<PasswordComplexityOptions> passwordOptions)
        {
            _passwordOptions = passwordOptions.Value;
        }

        /// <summary>
        /// Validates the specified password.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>A <see cref="Result"/> object containing the validation result, otherwise error messages.</returns>
        public Result Validate(string password)
        {
            var result = new Result();

            if (password.Length < _passwordOptions.MinimumLength)
            {
                result.Errors.Add(PasswordValidatonErrors.PasswordTooShort);
            }
            if (_passwordOptions.RequireUppercase && !Regex.IsMatch(password, @"[A-Z]"))
            {
                result.Errors.Add(PasswordValidatonErrors.PasswordRequiresUppercase);
            }
            if (_passwordOptions.RequireLowercase && !Regex.IsMatch(password, @"[a-z]"))
            {
                result.Errors.Add(PasswordValidatonErrors.PasswordRequiresLowercase);
            }
            if (_passwordOptions.RequireSpecialCharacter && !Regex.IsMatch(password, @"[\W_]"))
            {
                result.Errors.Add(PasswordValidatonErrors.PasswordRequiresSpecialCharacter);
            }

            return result;
        }
    }
}
