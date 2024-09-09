using Newtonsoft.Json;
using TaskManagement.Domain.Contracts.Logging;

namespace TaskManagement.Persistence.Extensions
{
    public static class LogEntryExtensions
    {
        public static string AsString(this LogEntry logEntry) => JsonConvert.SerializeObject(logEntry);
    }
}
