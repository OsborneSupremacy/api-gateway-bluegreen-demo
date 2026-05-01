namespace Ecommerce.Library.Utility;

/// <summary>
/// Represents the outcome of an operation without relying on exceptions for normal control flow.
/// </summary>
/// <typeparam name="T">The type of the successful result value.</typeparam>
/// <remarks>
/// This type models either:
/// <list type="bullet">
/// <item>
/// <description>A successful result with a <see cref="Value"/> and <see cref="StatusCode"/>.</description>
/// </item>
/// <item>
/// <description>A faulted result with an <see cref="Exception"/> and <see cref="StatusCode"/>.</description>
/// </item>
/// </list>
/// It is intended as a pragmatic stand-in until native discriminated unions are available in C#.
/// </remarks>
public class Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFaulted => !IsSuccess;

    /// <summary>
    /// Gets the HTTP status code associated with this result.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the successful result value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when accessed on a faulted result.
    /// </exception>
    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access value for faulted result");

    private readonly T? _value;

    /// <summary>
    /// Gets the exception that describes a faulted result.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when accessed on a successful result.
    /// </exception>
    public Exception Exception =>
        IsFaulted
            ? _exception!
            : throw new InvalidOperationException("Cannot access exception for successful result");

    private readonly Exception? _exception;

    /// <summary>
    /// Initializes a successful result.
    /// </summary>
    /// <param name="value">The successful value produced by the operation.</param>
    /// <param name="statusCode">The HTTP status code representing the successful outcome.</param>
    public Result(T value, HttpStatusCode statusCode)
    {
        _value = value;
        IsSuccess = true;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a faulted result.
    /// </summary>
    /// <param name="exception">The exception describing the failure.</param>
    /// <param name="statusCode">The HTTP status code representing the failure outcome.</param>
    public Result(Exception exception, HttpStatusCode statusCode)
    {
        _exception = exception;
        IsSuccess = false;
        StatusCode = statusCode;
    }
}
