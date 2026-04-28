using System.Collections.Immutable;

namespace Ecommerce.Library.Models;

public sealed record Order
{
    public required string OrderId { get; init; }

    public required string CustomerId { get; init; }

    public required string Currency { get; init; }

    public required string ShippingAddress { get; init; }

    public required ImmutableList<OrderLine> Items { get; init; }

    public required decimal TotalAmount { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}

public sealed record OrderLine
{
    public required string Sku { get; init; }

    public required string Name { get; init; }

    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }

    public decimal LineTotal { get; init; }
}