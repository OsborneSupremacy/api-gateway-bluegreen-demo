namespace Ecommerce.Api.Tests.Messaging;

public record OrderMetaData
{
    public required Guid OrderId { get; init; }

    public required Guid CustomerId { get; init; }
}