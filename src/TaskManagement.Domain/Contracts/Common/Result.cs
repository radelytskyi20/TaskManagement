namespace TaskManagement.Domain.Contracts.Common
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    public class Result : BaseResult
    {
        public static Result Success() => Success<Result>();
        public static Result Failure(params string[] errors) => Failure<Result>(errors);
        public static Result Forbid() => Forbid<Result>();
    }
}
