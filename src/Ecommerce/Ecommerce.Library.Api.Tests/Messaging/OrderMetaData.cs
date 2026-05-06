namespace Ecommerce.Library.Api.Tests.Messaging;

public record OrderMetaData
{
    public required Guid OrderId { get; init; }

    public required string CustomerId { get; init; }
}