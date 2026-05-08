namespace Ecommerce.Api.Tests.Models;

public sealed record DeleteOrderRequest
{
    public required Guid CustomerId { get; init; }

    public required Guid OrderId { get; init; }
}
