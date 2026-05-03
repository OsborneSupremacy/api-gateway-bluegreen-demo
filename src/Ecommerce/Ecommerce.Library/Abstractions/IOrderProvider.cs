using Ecommerce.Library.Models;

namespace Ecommerce.Library.Abstractions;

public interface IOrderProvider
{
    Task CreateAsync(Order order, CancellationToken cancellationToken);

    Task<Order?> GetOrder(string customerId, Guid orderId, CancellationToken cancellationToken);

    Task UpdateOrder(Order order, CancellationToken cancellationToken);

    Task DeleteOrder(string customerId, Guid orderId, CancellationToken cancellationToken);
}