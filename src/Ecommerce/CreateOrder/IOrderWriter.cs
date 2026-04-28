using Ecommerce.Library.Models;

namespace CreateOrder;

public interface IOrderWriter
{
    Task SaveAsync(Order order, CancellationToken cancellationToken);
}

