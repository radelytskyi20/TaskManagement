using TaskManagement.Domain.Contracts.Logging;

namespace TaskManagement.Persistence.Extensions
{
    /// <summary>
    /// Provides extension methods for building and modifying <see cref="LogEntry"/> instances.
    /// </summary>
    public static class LogEntryBuilder
    {
        public static LogEntry WithClass(this LogEntry logEntry, string @class)
        {
            logEntry.Class = @class;
            return logEntry;
        }

        public static LogEntry WithMethod(this LogEntry logEntry, string method)
        {
            logEntry.Method = method;
            return logEntry;
        }

        public static LogEntry WithComment(this LogEntry logEntry, string comment)
        {
            logEntry.Comment = comment;
            return logEntry;
        }

        public static LogEntry WithOperation(this LogEntry logEntry, string operation)
        {
            logEntry.Operation = operation;
            return logEntry;
        }

        public static LogEntry WithParametres(this LogEntry logEntry, string parametres)
        {
            logEntry.Parameters = parametres;
            return logEntry;
        }
    }
}
