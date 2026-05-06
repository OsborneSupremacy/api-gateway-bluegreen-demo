namespace Ecommerce.Order.Create.Models;

public sealed record CreateOrderRequest
{
    public required string CustomerId { get; init; }

    public required string Currency { get; init; }

    public required string ShippingAddress { get; init; }

    public required ImmutableList<CreateOrderItemRequest> Items { get; init; }
}

public sealed record CreateOrderItemRequest
{
    public required string Sku { get; init; }

    public required string Name { get; init; }

    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }
}

