namespace CreateOrder.Abstractions;

public interface IOrderWriter
{
    Task SaveAsync(Order order, CancellationToken cancellationToken);
}

