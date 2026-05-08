namespace Ecommerce.Order.Create.Models;

public sealed record CreateOrderRequest
{
    public required Guid CustomerId { get; init; }

    public required string Currency { get; init; }

    public required string ShippingAddress { get; init; }

    /// <summary>
    /// This new field represents a deliberate breaking change to a model that underlies
    /// the Create Order API contract. Making it required should cause tests to fail before
    /// the Lambda function is promoted to the blue environment.
    /// </summary>
    public required string CouponCode { get; init; }

    public required ImmutableList<CreateOrderItemRequest> Items { get; init; }
}

public sealed record CreateOrderItemRequest
{
    public required string Sku { get; init; }

    public required string Name { get; init; }

    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }
}

