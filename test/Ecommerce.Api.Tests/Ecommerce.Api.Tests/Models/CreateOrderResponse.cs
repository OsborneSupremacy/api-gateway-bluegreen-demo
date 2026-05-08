namespace Ecommerce.Api.Tests.Models;

public sealed record CreateOrderResponse
{
    public required Guid OrderId { get; init; }
}