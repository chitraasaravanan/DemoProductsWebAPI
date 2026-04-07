namespace DemoWebAPI.Core.Models
{
    /// <summary>
    /// Generic result wrapper for API responses supporting success and error states.
    /// </summary>
    /// <typeparam name="T">The type of the result data.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the result data when successful.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the error message when the operation fails.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets validation errors for the operation.
        /// </summary>
        public Dictionary<string, List<string>>? Errors { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Gets or sets the timestamp of the operation.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="data">The result data.</param>
        /// <param name="message">Optional success message.</param>
        /// <returns>A successful result instance.</returns>
        public static Result<T> Ok(T? data = default, string? message = null)
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>A failed result instance.</returns>
        public static Result<T> Fail(string message, int statusCode = 500)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a failed result with validation errors.
        /// </summary>
        /// <param name="errors">The validation errors.</param>
        /// <param name="message">The error message.</param>
        /// <returns>A failed result instance.</returns>
        public static Result<T> FailWithErrors(Dictionary<string, List<string>> errors, string? message = null)
        {
            return new Result<T>
            {
                Success = false,
                Message = message ?? "One or more validation errors occurred.",
                Errors = errors,
                StatusCode = 400
            };
        }
    }

    /// <summary>
    /// Generic result wrapper for non-generic API responses.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message associated with the operation.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Gets or sets the timestamp of the operation.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="message">Optional success message.</param>
        /// <returns>A successful result instance.</returns>
        public static Result Ok(string? message = null)
        {
            return new Result
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>A failed result instance.</returns>
        public static Result Fail(string message, int statusCode = 500)
        {
            return new Result
            {
                Success = false,
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}
