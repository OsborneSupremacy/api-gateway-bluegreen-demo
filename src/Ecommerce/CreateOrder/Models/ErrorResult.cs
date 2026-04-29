namespace CreateOrder.Models;

public sealed record ErrorResult
{
    public required string Message { get; init; }
}
