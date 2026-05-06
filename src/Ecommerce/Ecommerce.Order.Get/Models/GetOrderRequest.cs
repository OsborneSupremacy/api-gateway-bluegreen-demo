namespace Ecommerce.Order.Get.Models;

public sealed record GetOrderRequest
{
    public required string CustomerId { get; init; }

    public required Guid OrderId { get; init; }
}