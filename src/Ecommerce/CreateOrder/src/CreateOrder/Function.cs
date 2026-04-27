using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using System.Collections.Immutable;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateOrder;

public class Function
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IOrderWriter _orderWriter;

    public Function()
        : this(new DynamoDbOrderWriter(new AmazonDynamoDBClient(), ResolveOrdersTableName()))
    {
    }

    public Function(IOrderWriter orderWriter)
    {
        _orderWriter = orderWriter ?? throw new ArgumentNullException(nameof(orderWriter));
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(context);

        if (string.IsNullOrWhiteSpace(input.Body))
        {
            return BuildJsonResponse(400, new ErrorResult { Message = "Request body is required." });
        }

        CreateOrderRequest request;

        try
        {
            request = JsonSerializer.Deserialize<CreateOrderRequest>(input.Body, JsonOptions)
                ?? throw new JsonException("Request body is invalid.");
        }
        catch (JsonException)
        {
            return BuildJsonResponse(400, new ErrorResult { Message = "Request body JSON is invalid." });
        }

        var validationError = Validate(request);
        if (validationError.Length > 0)
        {
            return BuildJsonResponse(400, new ErrorResult { Message = validationError });
        }

        var order = BuildOrder(request);

        try
        {
            await _orderWriter.SaveAsync(order, CancellationToken.None);
        }
        catch (Exception ex)
        {
            context.Logger.Log($"CreateOrder failure for order '{order.OrderId}'. Error: {ex}");
            return BuildJsonResponse(500, new ErrorResult { Message = "Failed to persist order." });
        }

        return BuildJsonResponse(201, new CreateOrderResult { Order = order });
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

        return new Order
        {
            OrderId = $"ord_{Guid.NewGuid():N}",
            CustomerId = request.CustomerId.Trim(),
            Currency = request.Currency.Trim().ToUpperInvariant(),
            ShippingAddress = request.ShippingAddress.Trim(),
            Items = lines,
            TotalAmount = totalAmount,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    private static string Validate(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerId))
        {
            return "customerId is required.";
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            return "currency is required.";
        }

        if (string.IsNullOrWhiteSpace(request.ShippingAddress))
        {
            return "shippingAddress is required.";
        }

        if (request.Items.Count == 0)
        {
            return "At least one order item is required.";
        }

        if (request.Items.Any(item => string.IsNullOrWhiteSpace(item.Sku)))
        {
            return "Each item must include sku.";
        }

        if (request.Items.Any(item => string.IsNullOrWhiteSpace(item.Name)))
        {
            return "Each item must include name.";
        }

        if (request.Items.Any(item => item.Quantity <= 0))
        {
            return "Each item quantity must be greater than zero.";
        }

        if (request.Items.Any(item => item.UnitPrice <= 0))
        {
            return "Each item unitPrice must be greater than zero.";
        }

        return string.Empty;
    }

    private static APIGatewayProxyResponse BuildJsonResponse(int statusCode, object payload)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
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
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new InvalidOperationException("Environment variable ORDERS_TABLE_NAME is required.");
        }

        return tableName;
    }
}