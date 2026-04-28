using System.Collections.Immutable;
using Ecommerce.Library.Models;

namespace CreateOrder;







public sealed record CreateOrderResult
{
    public required Order Order { get; init; }
}

public sealed record ErrorResult
{
    public required string Message { get; init; }
}

