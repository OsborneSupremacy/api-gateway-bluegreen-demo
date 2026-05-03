using Ecommerce.Library.Models;

namespace Ecommerce.Library.Abstractions;

public interface IOrderProvider
{
    Task SaveAsync(Order order, CancellationToken cancellationToken);
}