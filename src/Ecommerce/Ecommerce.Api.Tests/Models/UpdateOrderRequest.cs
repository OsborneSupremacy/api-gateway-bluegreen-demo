namespace Ecommerce.Api.Tests.Models;

public sealed record UpdateOrderRequest
{
    public required string CustomerId { get; init; }

    public required Guid OrderId { get; init; }

    public required string Currency { get; init; }

    public required string ShippingAddress { get; init; }

    public required ImmutableList<UpdateOrderItemRequest> Items { get; init; }
}

public sealed record UpdateOrderItemRequest
{
    public required string Sku { get; init; }

    public required string Name { get; init; }

    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }
}
