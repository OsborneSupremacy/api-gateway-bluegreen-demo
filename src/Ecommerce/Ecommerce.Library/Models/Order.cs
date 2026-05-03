namespace Ecommerce.Library.Models;

public sealed record Order
{
    public required Guid OrderId { get; init; }

    public required string CustomerId { get; init; }

    public required string Currency { get; init; }

    public required string ShippingAddress { get; init; }

    public required ImmutableList<OrderLine> Items { get; init; }

    public required decimal TotalAmount { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}

public static class Orders
{
    public static Order Empty => new()
    {
        OrderId = Guid.Empty,
        CustomerId = string.Empty,
        Currency = string.Empty,
        ShippingAddress = string.Empty,
        Items = [],
        TotalAmount = 0m,
        CreatedAtUtc = DateTimeOffset.MinValue
    };
}

public sealed record OrderLine
{
    public required string Sku { get; init; }

    public required string Name { get; init; }

    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }

    public decimal LineTotal { get; init; }
}