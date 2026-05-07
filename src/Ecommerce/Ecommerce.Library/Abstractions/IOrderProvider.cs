using Ecommerce.Library.Models;

namespace Ecommerce.Library.Abstractions;

public interface IOrderProvider
{
    Task CreateOrderAsync(Order order, CancellationToken cancellationToken);

    Task<Order> GetOrderAsync(Guid CustomerId, Guid orderId, CancellationToken cancellationToken);

    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);

    Task DeleteOrderAsync(Guid CustomerId, Guid orderId, CancellationToken cancellationToken);
}