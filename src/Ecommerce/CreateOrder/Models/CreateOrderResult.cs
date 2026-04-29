namespace CreateOrder.Models;

public sealed record CreateOrderResult
{
    public required Order Order { get; init; }
}