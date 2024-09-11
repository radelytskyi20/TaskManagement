namespace TaskManagement.Domain.Contracts.Logging
{
    /// <summary>
    /// Represents a log entry containing details about a specific operation or event.
    /// </summary>
    public class LogEntry
    {
        public string Class { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
    }
}
