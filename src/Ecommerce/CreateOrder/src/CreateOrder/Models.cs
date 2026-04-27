using System.Collections.Immutable;

namespace CreateOrder;

public sealed record CreateOrderItemRequest
{
    public required string Sku { get; init; }

    public required string Name { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }
}

public sealed record CreateOrderRequest
{
    public required string CustomerId { get; init; }

    public required string Currency { get; init; }

    public required string ShippingAddress { get; init; }

    public ImmutableList<CreateOrderItemRequest> Items { get; init; } = ImmutableList<CreateOrderItemRequest>.Empty;
}

public sealed record OrderLine
{
    public required string Sku { get; init; }

    public required string Name { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal LineTotal { get; init; }
}

public sealed record Order
{
    public required string OrderId { get; init; }

    public required string CustomerId { get; init; }

    public required string Currency { get; init; }

    public required string ShippingAddress { get; init; }

    public ImmutableList<OrderLine> Items { get; init; } = ImmutableList<OrderLine>.Empty;

    public decimal TotalAmount { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }
}

public sealed record CreateOrderResult
{
    public required Order Order { get; init; }
}

public sealed record ErrorResult
{
    public required string Message { get; init; }
}

