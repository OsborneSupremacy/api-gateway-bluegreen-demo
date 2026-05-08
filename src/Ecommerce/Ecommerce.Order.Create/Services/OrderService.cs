using Amazon.Lambda.APIGatewayEvents;

namespace Ecommerce.Order.Create.Services;

internal class OrderService : IApiGatewayHandler
{
    private readonly ApiGatewayAdapter _adapter;

    private readonly AbstractValidator<CreateOrderRequest> _validator;

    private readonly IOrderProvider _orderProvider;

    public OrderService(
        ApiGatewayAdapter adapter,
        AbstractValidator<CreateOrderRequest> validator,
        IOrderProvider orderProvider
        )
    {
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _orderProvider = orderProvider ?? throw new ArgumentNullException(nameof(orderProvider));
    }

    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request) =>
        _adapter
            .AdaptAsync<CreateOrderRequest, CreateOrderResponse>(request, CreateOrderAsync);

    internal async Task<Result<CreateOrderResponse>> CreateOrderAsync(CreateOrderRequest request)
    {
        var validationResult = await _validator
            .ValidateAsync(request, CancellationToken.None)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
            return new Result<CreateOrderResponse>(
                new ValidationException(string.Join(" ", validationResult.Errors)),
                HttpStatusCode.BadRequest
            );

        var order = BuildOrder(request);

        await _orderProvider
            .CreateOrderAsync(order, CancellationToken.None)
            .ConfigureAwait(false);

        return new Result<CreateOrderResponse>(new CreateOrderResponse { OrderId = order.OrderId }, HttpStatusCode.Created);
    }

    private static Library.Models.Order BuildOrder(CreateOrderRequest request)
    {
        var lines = request.Items
            .Select(item => new OrderLine
            {
                Sku = item.Sku.Trim(),
                Name = item.Name.Trim(),
                Quantity = item.Quantity,
                UnitPrice = decimal.Round(item.UnitPrice, 2, MidpointRounding.AwayFromZero),
                LineTotal = decimal.Round(item.UnitPrice * item.Quantity, 2, MidpointRounding.AwayFromZero)
            })
            .ToImmutableList();

        var totalAmount = decimal.Round(lines.Sum(line => line.LineTotal), 2, MidpointRounding.AwayFromZero);

        var orderId = $"{Guid.CreateVersion7():N}";

        return new Library.Models.Order
        {
            OrderId = Guid.Parse(orderId),
            CustomerId = request.CustomerId,
            Currency = request.Currency.Trim().ToUpperInvariant(),
            CouponCode = request.CouponCode.Trim().ToUpperInvariant(),
            ShippingAddress = request.ShippingAddress.Trim(),
            Items = lines,
            TotalAmount = totalAmount,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }
}