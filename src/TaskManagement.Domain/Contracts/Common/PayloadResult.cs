namespace TaskManagement.Domain.Contracts.Common
{
    /// <summary>
    /// Represents a result that contains a payload of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the payload.</typeparam>
    public class PayloadResult<T> : Result
    {
        /// <summary>
        /// Gets the payload of the result.
        /// </summary>
        public T? Payload { get; protected set; }

        /// <summary>
        /// Creates a successful <see cref="PayloadResult{T}"/> with the specified payload.
        /// </summary>
        /// <param name="payload">The payload of the result.</param>
        /// <returns>A successful <see cref="PayloadResult{T}"/>.</returns>
        public static PayloadResult<T> Success(T payload) => new() { Payload = payload };

        /// <summary>
        /// Creates a failed <see cref="PayloadResult{T}"/> with the specified errors.
        /// </summary>
        /// <param name="errors">The errors that caused the failure.</param>
        /// <returns>A failed <see cref="PayloadResult{T}"/>.</returns>
        public new static PayloadResult<T> Failure(params string[] errors) => new() { Errors = errors.ToList() };
    }

}
