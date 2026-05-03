using Amazon.Lambda.APIGatewayEvents;
using Ecommerce.Library.Utility;

namespace GetOrder.Services;

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

    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request) =>
        _adapter
            .AdaptAsync<GetOrderRequest, GetOrderResponse>(request, GetOrderAsync);

    internal async Task<Result<GetOrderResponse>> GetOrderAsync(GetOrderRequest request)
    {
        var validationResult = await _validator
            .ValidateAsync(request, CancellationToken.None)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            return new Result<GetOrderResponse>(
                new InvalidOperationException(string.Join(" ", validationResult.Errors)),
                HttpStatusCode.BadRequest);
        }

        var order = await _orderProvider
            .GetOrderAsync(request.CustomerId.Trim(), request.OrderId, CancellationToken.None)
            .ConfigureAwait(false);

        if (order.OrderId == Guid.Empty)
        {
            return new Result<GetOrderResponse>(
                new InvalidOperationException("Order not found."),
                HttpStatusCode.NotFound);
        }

        return new Result<GetOrderResponse>(MapResponse(order), HttpStatusCode.OK);
    }

    private static GetOrderResponse MapResponse(Order order) =>
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