namespace TaskManagement.Domain.Contracts.Auth
{
    /// <summary>
    /// Represents the options object for password complexity requirements settings.
    /// </summary>
    public class PasswordOptions
    {
        public const string SectionName = nameof(PasswordOptions);

        public int MinimumLength { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireSpecialCharacter { get; set; }
    }
}
