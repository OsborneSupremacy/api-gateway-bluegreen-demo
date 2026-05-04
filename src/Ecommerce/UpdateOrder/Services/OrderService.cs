using Amazon.Lambda.APIGatewayEvents;
using Ecommerce.Library.Extensions;
using Ecommerce.Library.Utility;
using Ecommerce.Library.Services;

namespace UpdateOrder.Services;

internal class OrderService : IApiGatewayHandler
{
    private readonly ApiGatewayAdapter _adapter;

    private readonly JsonService _jsonService;

    private readonly AbstractValidator<UpdateOrderRequest> _validator;

    private readonly IOrderProvider _orderProvider;

    public OrderService(
        ApiGatewayAdapter adapter,
        JsonService jsonService,
        AbstractValidator<UpdateOrderRequest> validator,
        IOrderProvider orderProvider)
    {
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        _jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _orderProvider = orderProvider ?? throw new ArgumentNullException(nameof(orderProvider));
    }

    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request) =>
        _adapter.AdaptAsync<UpdateOrderRequest, UpdateOrderResponse>(request, UpdateOrderAsync);

    // ReSharper disable once MemberCanBePrivate.Global
    internal async Task<Result<UpdateOrderResponse>> UpdateOrderAsync(UpdateOrderRequest request)
    {
        var validationResult = await _validator
            .ValidateAsync(request, CancellationToken.None)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
            return new Result<UpdateOrderResponse>(
                new ValidationException(string.Join(" ", validationResult.Errors)),
                HttpStatusCode.BadRequest
            );

        var existing = await _orderProvider
            .GetOrderAsync(request.CustomerId.Trim(), request.OrderId, CancellationToken.None)
            .ConfigureAwait(false);

        if (existing == Orders.Empty)
            return new Result<UpdateOrderResponse>(
                new InvalidOperationException("Order not found."),
                HttpStatusCode.NotFound
            );

        var updatedOrder = BuildOrder(request, existing);

        await _orderProvider
            .UpdateOrderAsync(updatedOrder, CancellationToken.None)
            .ConfigureAwait(false);

        return new Result<UpdateOrderResponse>(MapResponse(updatedOrder), HttpStatusCode.OK);
    }

    private static Order BuildOrder(UpdateOrderRequest request, Order existing)
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

        return existing with
        {
            Currency = request.Currency.Trim().ToUpperInvariant(),
            ShippingAddress = request.ShippingAddress.Trim(),
            Items = lines,
            TotalAmount = totalAmount
        };
    }

    private static UpdateOrderResponse MapResponse(Order order) =>
        new()
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            Currency = order.Currency,
            ShippingAddress = order.ShippingAddress,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Items = order.Items
                .Select(item => new UpdateOrderItemResponse
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
