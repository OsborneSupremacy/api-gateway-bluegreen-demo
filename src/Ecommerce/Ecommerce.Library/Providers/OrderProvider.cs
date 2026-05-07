using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using dotenv.net.Utilities;
using Ecommerce.Library.Abstractions;
using Ecommerce.Library.Models;

namespace Ecommerce.Library.Providers;

public sealed class OrderProvider : IOrderProvider
{
    private readonly ILogger<OrderProvider> _logger;

    private readonly IAmazonDynamoDB _dynamoDbClient;

    private readonly string _tableName;

    public OrderProvider(
        ILogger<OrderProvider> logger,
        IAmazonDynamoDB dynamoDb
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dynamoDbClient = dynamoDb ?? throw new ArgumentNullException(nameof(dynamoDb));
        _tableName = EnvReader.GetStringValue("ORDERS_TABLE_NAME");
    }

    public async Task CreateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        var ttl = order.CreatedAtUtc
            .AddDays(1)
            .ToUnixTimeSeconds()
            .ToString(System.Globalization.CultureInfo.InvariantCulture);

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["PK"] = new() { S = $"CUSTOMER#{order.CustomerId}#ORDER" },
                ["SK"] = new() { S = $"ORDER#{order.OrderId}" },
                ["OrderId"] = new() { S = order.OrderId.ToString() },
                ["CustomerId"] = new() { S = order.CustomerId.ToString() },
                ["Currency"] = new() { S = order.Currency },
                ["ShippingAddress"] = new() { S = order.ShippingAddress },
                ["TotalAmount"] = new() { N = order.TotalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
                ["CreatedAtUtc"] = new() { S = order.CreatedAtUtc.ToString("O") },
                ["ttl"] = new() { N = ttl },
                ["Items"] = new()
                {
                    L =
                    [
                        .. order.Items.Select(item => new AttributeValue
                        {
                            M = new Dictionary<string, AttributeValue>
                            {
                                ["Sku"] = new() { S = item.Sku },
                                ["Name"] = new() { S = item.Name },
                                ["Quantity"] = new() { N = item.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture) },
                                ["UnitPrice"] = new() { N = item.UnitPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
                                ["LineTotal"] = new() { N = item.LineTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) }
                            }
                        })
                    ]
                }
            }
        };

        await _dynamoDbClient
            .PutItemAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Order> GetOrderAsync(Guid customerId, Guid orderId, CancellationToken cancellationToken)
    {
        var pk = BuildPartitionKey(customerId);
        var sk = BuildSortKey(orderId);

        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["PK"] = new() { S = pk },
                ["SK"] = new() { S = sk }
            }
        };

        var response = await _dynamoDbClient
            .GetItemAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return (response?.Item?.Count ?? 0) == 0 ? Orders.Empty : MapOrder(response!.Item!);
    }

    public async Task UpdateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        var request = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["PK"] = new() { S = BuildPartitionKey(order.CustomerId) },
                ["SK"] = new() { S = BuildSortKey(order.OrderId) }
            },
            UpdateExpression = "SET #orderId = :orderId, #customerId = :customerId, #currency = :currency, #shippingAddress = :shippingAddress, #totalAmount = :totalAmount, #createdAtUtc = :createdAtUtc, #items = :items",
            ConditionExpression = "attribute_exists(PK) AND attribute_exists(SK)",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                ["#orderId"] = "OrderId",
                ["#customerId"] = "CustomerId",
                ["#currency"] = "Currency",
                ["#shippingAddress"] = "ShippingAddress",
                ["#totalAmount"] = "TotalAmount",
                ["#createdAtUtc"] = "CreatedAtUtc",
                ["#items"] = "Items"
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":orderId"] = new() { S = order.OrderId.ToString() },
                [":customerId"] = new() { S = order.CustomerId.ToString() },
                [":currency"] = new() { S = order.Currency },
                [":shippingAddress"] = new() { S = order.ShippingAddress },
                [":totalAmount"] = new() { N = order.TotalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
                [":createdAtUtc"] = new() { S = order.CreatedAtUtc.ToString("O") },
                [":items"] = new()
                {
                    L =
                    [
                        .. order.Items.Select(item => new AttributeValue
                        {
                            M = new Dictionary<string, AttributeValue>
                            {
                                ["Sku"] = new() { S = item.Sku },
                                ["Name"] = new() { S = item.Name },
                                ["Quantity"] = new() { N = item.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture) },
                                ["UnitPrice"] = new() { N = item.UnitPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
                                ["LineTotal"] = new() { N = item.LineTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) }
                            }
                        })
                    ]
                }
            }
        };

        await _dynamoDbClient
            .UpdateItemAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task DeleteOrderAsync(Guid customerId, Guid orderId, CancellationToken cancellationToken)
    {
        var request = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["PK"] = new() { S = BuildPartitionKey(customerId) },
                ["SK"] = new() { S = BuildSortKey(orderId) }
            }
        };

        await _dynamoDbClient
            .DeleteItemAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }

    private static string BuildPartitionKey(Guid customerId) =>
        $"CUSTOMER#{customerId}#ORDER";

    private static string BuildSortKey(Guid orderId) =>
        $"ORDER#{orderId}";

    private static Order MapOrder(Dictionary<string, AttributeValue> item)
    {
        var orderId = Guid.Parse(GetRequiredString(item, "OrderId"));
        var customerId = Guid.Parse(GetRequiredString(item, "CustomerId"));
        var currency = GetRequiredString(item, "Currency");
        var shippingAddress = GetRequiredString(item, "ShippingAddress");
        var totalAmount = decimal.Parse(GetRequiredNumber(item, "TotalAmount"), System.Globalization.CultureInfo.InvariantCulture);
        var createdAtUtc = DateTimeOffset.Parse(GetRequiredString(item, "CreatedAtUtc"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);

        var orderLines = GetRequiredList(item, "Items")
            .Select(orderLine => orderLine.M)
            .Select(orderLineMap => new OrderLine
            {
                Sku = GetRequiredString(orderLineMap, "Sku"),
                Name = GetRequiredString(orderLineMap, "Name"),
                Quantity = int.Parse(GetRequiredNumber(orderLineMap, "Quantity"), System.Globalization.CultureInfo.InvariantCulture),
                UnitPrice = decimal.Parse(GetRequiredNumber(orderLineMap, "UnitPrice"), System.Globalization.CultureInfo.InvariantCulture),
                LineTotal = decimal.Parse(GetRequiredNumber(orderLineMap, "LineTotal"), System.Globalization.CultureInfo.InvariantCulture)
            })
            .ToImmutableList();

        return new Order
        {
            OrderId = orderId,
            CustomerId = customerId,
            Currency = currency,
            ShippingAddress = shippingAddress,
            TotalAmount = totalAmount,
            CreatedAtUtc = createdAtUtc,
            Items = orderLines
        };
    }

    private static string GetRequiredString(Dictionary<string, AttributeValue> item, string key)
    {
        if (!item.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value.S))
            throw new InvalidOperationException($"Missing or invalid string attribute '{key}'.");

        return value.S;
    }

    private static string GetRequiredNumber(Dictionary<string, AttributeValue> item, string key)
    {
        if (!item.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value.N))
            throw new InvalidOperationException($"Missing or invalid number attribute '{key}'.");

        return value.N;
    }

    private static List<AttributeValue> GetRequiredList(Dictionary<string, AttributeValue> item, string key)
    {
        if (!item.TryGetValue(key, out var value) || value.L is null)
            throw new InvalidOperationException($"Missing or invalid list attribute '{key}'.");

        return value.L;
    }
}