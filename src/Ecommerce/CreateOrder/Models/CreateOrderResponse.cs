namespace CreateOrder.Models;

public sealed record CreateOrderResponse
{
    public required Guid OrderId { get; init; }
}