namespace DeleteOrder.Models;

public sealed record DeleteOrderRequest
{
    public required string CustomerId { get; init; }

    public required Guid OrderId { get; init; }
}
