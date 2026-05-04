using Amazon.Lambda.APIGatewayEvents;
using Ecommerce.Library.Messaging;
using Ecommerce.Library.Utility;

namespace DeleteOrder.Services;

internal class OrderService : IApiGatewayHandler
{
    private readonly ApiGatewayAdapter _adapter;

    private readonly AbstractValidator<DeleteOrderRequest> _validator;

    private readonly IOrderProvider _orderProvider;

    public OrderService(
        ApiGatewayAdapter adapter,
        AbstractValidator<DeleteOrderRequest> validator,
        IOrderProvider orderProvider)
    {
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _orderProvider = orderProvider ?? throw new ArgumentNullException(nameof(orderProvider));
    }

    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request) =>
        _adapter.AdaptAsync<DeleteOrderRequest, StatusCodeOnlyResponse>(request, DeleteOrderAsync);

    internal async Task<Result<StatusCodeOnlyResponse>> DeleteOrderAsync(DeleteOrderRequest request)
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

        await _orderProvider
            .DeleteOrderAsync(request.CustomerId.Trim(), request.OrderId, CancellationToken.None)
            .ConfigureAwait(false);

        return new Result<StatusCodeOnlyResponse>(
            new StatusCodeOnlyResponse { StatusCode = HttpStatusCode.OK },
            HttpStatusCode.OK
        );
    }
}
