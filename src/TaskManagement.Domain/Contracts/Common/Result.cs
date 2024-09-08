namespace TaskManagement.Domain.Contracts.Common
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool Succeeded => Errors.Count == 0;

        /// <summary>
        /// Gets the collection of errors associated with the operation.
        /// </summary>
        public ICollection<string> Errors { get; protected set; } = new List<string>();

        /// <summary>
        /// Creates a new instance of the <see cref="Result"/> class representing a successful operation.
        /// </summary>
        /// <returns>A new instance of the <see cref="Result"/> class.</returns>
        public static Result Success() => new();

        /// <summary>
        /// Creates a new instance of the <see cref="Result"/> class representing a failed operation with the specified errors.
        /// </summary>
        /// <param name="errors">The errors associated with the failed operation.</param>
        /// <returns>A new instance of the <see cref="Result"/> class.</returns>
        public static Result Failure(params string[] errors) => new() { Errors = errors.ToList() };
    }
}
