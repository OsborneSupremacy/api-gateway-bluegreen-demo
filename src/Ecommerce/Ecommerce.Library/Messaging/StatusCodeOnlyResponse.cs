namespace Ecommerce.Library.Messaging;

public record StatusCodeOnlyResponse
{
    public required HttpStatusCode StatusCode { get; init; }
}
