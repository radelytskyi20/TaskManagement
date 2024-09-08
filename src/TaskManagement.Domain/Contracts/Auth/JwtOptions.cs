namespace TaskManagement.Domain.Contracts.Auth
{
    /// <summary>
    /// Represents the options object for jwt token settings.
    /// </summary>
    public class JwtOptions
    {
        public const string SectionName = nameof(JwtOptions);
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int ExpriesMinutes { get; set; }
    }
}
