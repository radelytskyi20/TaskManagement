namespace TaskManagement.Domain.Contracts.Common
{
    /// <summary>
    /// Represents a result that contains a payload of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the payload.</typeparam>
    public class PayloadResult<T> : BaseResult
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
        public static PayloadResult<T> Failure(params string[] errors) => Failure<PayloadResult<T>>(errors);
        public static PayloadResult<T> Forbid() => Forbid<PayloadResult<T>>();
    }

}
