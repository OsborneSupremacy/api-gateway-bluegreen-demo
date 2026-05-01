using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using System.Text.Json;
using CreateOrder.Validators;
using FluentValidation;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateOrder;

public class Function
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IOrderProvider _orderProvider;

    private readonly IValidator<CreateOrderRequest> _validator;

    public Function()
        : this(new OrderProvider(new AmazonDynamoDBClient(), ResolveOrdersTableName()),
               new CreateOrderRequestValidator())
    {
    }

    public Function(IOrderProvider orderProvider, IValidator<CreateOrderRequest> validator)
    {
        _orderProvider = orderProvider ?? throw new ArgumentNullException(nameof(orderProvider));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(
        APIGatewayProxyRequest input,
        ILambdaContext context
        )
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(context);

        CreateOrderRequest request;

        try
        {
            request = JsonSerializer.Deserialize<CreateOrderRequest>(input.Body, JsonOptions)
                ?? throw new JsonException("Request body is invalid.");
        }
        catch (JsonException)
        {
            return BuildJsonResponse(HttpStatusCode.BadRequest, new ErrorResult { Message = "Request body JSON is invalid." });
        }

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BuildJsonResponse(HttpStatusCode.BadRequest,
                new ErrorResult { Message = validationResult.Errors[0].ErrorMessage });
        }

        var order = BuildOrder(request);

        try
        {
            await _orderProvider.SaveAsync(order, CancellationToken.None);
        }
        catch (Exception ex)
        {
            context.Logger.Log($"CreateOrder failure for order '{order.OrderId}'. Error: {ex}");
            return BuildJsonResponse(HttpStatusCode.InternalServerError, new ErrorResult { Message = "Failed to persist order." });
        }

        return BuildJsonResponse(HttpStatusCode.Created, new CreateOrderResponse { OrderId = order.OrderId });
    }

    private static Order BuildOrder(CreateOrderRequest request)
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

        return new Order
        {
            OrderId = Guid.Parse(orderId),
            CustomerId = request.CustomerId.Trim(),
            Currency = request.Currency.Trim().ToUpperInvariant(),
            ShippingAddress = request.ShippingAddress.Trim(),
            Items = lines,
            TotalAmount = totalAmount,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }


    private static APIGatewayProxyResponse BuildJsonResponse(HttpStatusCode statusCode, object payload)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = (int)statusCode,
            Body = JsonSerializer.Serialize(payload, JsonOptions),
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json"
            }
        };
    }

    private static string ResolveOrdersTableName()
    {
        var tableName = Environment.GetEnvironmentVariable("ORDERS_TABLE_NAME");
        return string.IsNullOrWhiteSpace(tableName) ? throw new InvalidOperationException("Environment variable ORDERS_TABLE_NAME is required.") : tableName;
    }
}