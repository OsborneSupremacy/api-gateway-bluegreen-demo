using Ecommerce.Library.Models;

namespace Ecommerce.Library.Abstractions;

public interface IOrderProvider
{
    Task CreateAsync(Order order, CancellationToken cancellationToken);

    Task<Order> GetOrderAsync(string customerId, Guid orderId, CancellationToken cancellationToken);

    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);

    Task DeleteOrderAsync(string customerId, Guid orderId, CancellationToken cancellationToken);
}