namespace Ecommerce.Library.Messaging;

/// <summary>
/// A simple error record consistent t with API Gateway's default error response structure.
/// </summary>
internal record ErrorResponse
{
    public required string Message { get; init; }
}
