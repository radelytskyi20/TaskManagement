namespace TaskManagement.Domain.Constants
{
    public static class PasswordValidatonErrors
    {
        public const string PasswordTooShort = "Password is too short.";
        public const string PasswordRequiresUppercase = "Password requires uppercase letter.";
        public const string PasswordRequiresLowercase = "Password requires lowercase letter.";
        public const string PasswordRequiresDigit = "Password requires digit.";
        public const string PasswordRequiresSpecialCharacter = "Password requires special character.";
    }
}
