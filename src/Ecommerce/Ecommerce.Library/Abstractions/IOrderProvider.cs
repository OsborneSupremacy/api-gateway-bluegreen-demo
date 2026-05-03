using Ecommerce.Library.Models;

namespace Ecommerce.Library.Abstractions;

public interface IOrderProvider
{
    Task CreateAsync(Order order, CancellationToken cancellationToken);
}