using Amazon.Lambda.APIGatewayEvents;

namespace Ecommerce.Order.Update.Services;

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
        _adapter.AdaptAsync<UpdateOrderRequest, StatusCodeOnlyResponse>(request, UpdateOrderAsync);

    // ReSharper disable once MemberCanBePrivate.Global
    internal async Task<Result<StatusCodeOnlyResponse>> UpdateOrderAsync(UpdateOrderRequest request)
    {
        var validationResult = await _validator
            .ValidateAsync(request, CancellationToken.None)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
            return new Result<StatusCodeOnlyResponse>(
                new ValidationException(string.Join(" ", validationResult.Errors)),
                HttpStatusCode.BadRequest
            );

        var existing = await _orderProvider
            .GetOrderAsync(request.CustomerId.Trim(), request.OrderId, CancellationToken.None)
            .ConfigureAwait(false);

        if (existing == Orders.Empty)
            return new Result<StatusCodeOnlyResponse>(
                new InvalidOperationException("Order not found."),
                HttpStatusCode.NotFound
            );

        var updatedOrder = BuildOrder(request, existing);

        await _orderProvider
            .UpdateOrderAsync(updatedOrder, CancellationToken.None)
            .ConfigureAwait(false);

        return new Result<StatusCodeOnlyResponse>(
            new StatusCodeOnlyResponse { StatusCode = HttpStatusCode.OK },
            HttpStatusCode.OK
        );
    }

    private static Library.Models.Order BuildOrder(UpdateOrderRequest request, Library.Models.Order existing)
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
}
