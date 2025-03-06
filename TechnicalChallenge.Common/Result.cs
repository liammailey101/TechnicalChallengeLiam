using System.Diagnostics.CodeAnalysis;

namespace TechnicalChallenge.Common
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="error">The error associated with the operation, if any.</param>
        /// <exception cref="InvalidOperationException">Thrown when the success state and error state are inconsistent.</exception>
        protected Result(bool isSuccess, ResultError error)
        {
            switch (isSuccess)
            {
                case true when error != ResultError.None:
                    throw new InvalidOperationException();

                case false when error == ResultError.None:
                    throw new InvalidOperationException();

                default:
                    IsSuccess = isSuccess;
                    Error = error;
                    break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the error associated with the operation, if any.
        /// </summary>
        public ResultError Error { get; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>A successful result.</returns>
        /// <example>
        /// var result = Result.Success();
        /// if (result.IsSuccess)
        /// {
        ///     // Operation was successful
        /// }
        /// </example>
        public static Result Success() => new(true, ResultError.None);

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        /// <param name="error">The error associated with the failure.</param>
        /// <returns>A failed result.</returns>
        /// <example>
        /// var result = Result.Failure(new ResultError("Error.Code", "Error message"));
        /// if (result.IsFailure)
        /// {
        ///     // Operation failed
        /// }
        /// </example>
        public static Result Failure(ResultError error) => new(false, error);

        /// <summary>
        /// Creates a successful result with the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value associated with the success.</param>
        /// <returns>A successful result with the specified value.</returns>
        /// <example>
        /// var result = Result.Success(42);
        /// if (result.IsSuccess)
        /// {
        ///     var value = result.Value;
        ///     // Use the value
        /// }
        /// </example>
        public static Result<T> Success<T>(T value) => new(value, true, ResultError.None);

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="error">The error associated with the failure.</param>
        /// <returns>A failed result.</returns>
        /// <example>
        /// var result = Result.Failure<int>(new ResultError("Error.Code", "Error message"));
        /// if (result.IsFailure)
        /// {
        ///     // Operation failed
        /// }
        /// </example>
        public static Result<T> Failure<T>(ResultError error) => new(default, false, error);

        /// <summary>
        /// Creates a result based on the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to create the result from.</param>
        /// <returns>A successful result if the value is not null; otherwise, a failed result.</returns>
        /// <example>
        /// var result = Result.Create(42);
        /// if (result.IsSuccess)
        /// {
        ///     var value = result.Value;
        ///     // Use the value
        /// }
        /// </example>
        public static Result<T> Create<T>(T? value) => value is not null ? Success(value) : Failure<T>(ResultError.NullValue);
    }

    /// <summary>
    /// Represents the result of an operation with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class Result<T> : Result
    {
        private readonly T? _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="value">The value associated with the result.</param>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="error">The error associated with the operation, if any.</param>
        protected internal Result(T? value, bool isSuccess, ResultError error) : base(isSuccess, error)
            => _value = value;

        /// <summary>
        /// Gets the value associated with the result.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the result has no value.</exception>
        [NotNull]
        public T Value => _value! ?? throw new InvalidOperationException("Result has no value");

        /// <summary>
        /// Implicitly converts a value to a result.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A result containing the value.</returns>
        public static implicit operator Result<T>(T? value) => Create(value);
    }

    /// <summary>
    /// Represents an error associated with a result.
    /// </summary>
    /// <param name="Code">The error code.</param>
    /// <param name="Message">The error message.</param>
    public record ResultError(string Code, string Message)
    {
        /// <summary>
        /// Represents no error.
        /// </summary>
        public static ResultError None = new(string.Empty, string.Empty);

        /// <summary>
        /// Represents a record not found error.
        /// </summary>
        public static ResultError RecordNotFound = new("Error.RecordNotFound", "Record not found.");

        /// <summary>
        /// Represents a null value error.
        /// </summary>
        public static ResultError NullValue = new("Error.NullValue", "A null value was provided.");
    }
}