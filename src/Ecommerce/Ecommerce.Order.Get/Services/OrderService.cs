using Amazon.Lambda.APIGatewayEvents;

namespace Ecommerce.Order.Get.Services;

internal class OrderService : IApiGatewayHandler
{
    private readonly ApiGatewayAdapter _adapter;

    private readonly AbstractValidator<GetOrderRequest> _validator;

    private readonly IOrderProvider _orderProvider;

    public OrderService(
        ApiGatewayAdapter adapter,
        AbstractValidator<GetOrderRequest> validator,
        IOrderProvider orderProvider)
    {
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _orderProvider = orderProvider ?? throw new ArgumentNullException(nameof(orderProvider));
    }

    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request)
    {
        var getOrderRequest = new GetOrderRequest
        {
            OrderId = Guid.TryParse(request.PathParameters["orderId"], out var orderid) ? orderid : Guid.Empty,
            CustomerId = Guid.TryParse(request.PathParameters["customerId"], out var customerId) ? customerId : Guid.Empty
        };

        return _adapter
            .AdaptAsync(getOrderRequest, GetOrderAsync);
    }

    internal async Task<Result<GetOrderResponse>> GetOrderAsync(GetOrderRequest request)
    {
        var validationResult = await _validator
            .ValidateAsync(request, CancellationToken.None)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
            return new Result<GetOrderResponse>(
                new ValidationException(string.Join(" ", validationResult.Errors)),
                HttpStatusCode.BadRequest
            );

        var order = await _orderProvider
            .GetOrderAsync(request.CustomerId, request.OrderId, CancellationToken.None)
            .ConfigureAwait(false);

        if (order == Orders.Empty)
            return new Result<GetOrderResponse>(
                new InvalidOperationException("Order not found."),
                HttpStatusCode.NotFound
            );

        return new Result<GetOrderResponse>(MapResponse(order), HttpStatusCode.OK);
    }

    private static GetOrderResponse MapResponse(Library.Models.Order order) =>
        new()
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            Currency = order.Currency,
            ShippingAddress = order.ShippingAddress,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Items = order.Items
                .Select(item => new GetOrderItemResponse
                {
                    Sku = item.Sku,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.LineTotal
                })
                .ToImmutableList()
        };
}