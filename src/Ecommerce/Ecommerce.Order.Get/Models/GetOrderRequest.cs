namespace Ecommerce.Order.Get.Models;

public sealed record GetOrderRequest
{
    public required Guid CustomerId { get; init; }

    public required Guid OrderId { get; init; }
}