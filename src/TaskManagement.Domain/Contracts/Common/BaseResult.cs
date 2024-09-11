namespace TaskManagement.Domain.Contracts.Common
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    public abstract class BaseResult
    {
        /// <summary>
        /// Gets a value indicating whether the operation is forbidden.
        /// </summary>
        public bool IsForbiden { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool Succeeded => Errors.Count == 0;

        /// <summary>
        /// Gets the collection of errors associated with the operation.
        /// </summary>
        public ICollection<string> Errors { get; protected set; } = new List<string>();

        public static T Success<T>() where T : BaseResult, new() => new();
        public static T Failure<T>(params string[] errors) where T : BaseResult, new() => new() { Errors = errors.ToList() };
        public static T Forbid<T>() where T : BaseResult, new() => new() { IsForbiden = true };
    }
}
