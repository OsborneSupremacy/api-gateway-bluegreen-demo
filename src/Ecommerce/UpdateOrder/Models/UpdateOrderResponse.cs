namespace UpdateOrder.Models;

public sealed record UpdateOrderResponse
{
    public required Guid OrderId { get; init; }

    public required string CustomerId { get; init; }

    public required string Currency { get; init; }

    public required string ShippingAddress { get; init; }

    public required ImmutableList<UpdateOrderItemResponse> Items { get; init; }

    public required decimal TotalAmount { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}

public sealed record UpdateOrderItemResponse
{
    public required string Sku { get; init; }

    public required string Name { get; init; }

    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }

    public required decimal LineTotal { get; init; }
}
