namespace CreateOrder.Abstractions;

public interface IOrderProvider
{
    Task SaveAsync(Order order, CancellationToken cancellationToken);
}

