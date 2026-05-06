namespace Ecommerce.Order.Create.Models;

public sealed record CreateOrderResponse
{
    public required Guid OrderId { get; init; }
}