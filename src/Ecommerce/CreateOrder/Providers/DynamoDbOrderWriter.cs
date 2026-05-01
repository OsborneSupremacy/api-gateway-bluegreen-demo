using Amazon.DynamoDBv2;

namespace CreateOrder.Providers;

public sealed class DynamoDbOrderWriter : IOrderWriter
{
    private readonly IAmazonDynamoDB _dynamoDb;

    private readonly string _tableName;

    public DynamoDbOrderWriter(IAmazonDynamoDB dynamoDb, string tableName)
    {
        _dynamoDb = dynamoDb ?? throw new ArgumentNullException(nameof(dynamoDb));
        _tableName = string.IsNullOrWhiteSpace(tableName)
            ? throw new ArgumentException("A DynamoDB table name must be configured.", nameof(tableName))
            : tableName;
    }

    public async Task SaveAsync(Order order, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["PK"] = new() { S = $"CUSTOMER#{order.CustomerId}#ORDER" },
                ["SK"] = new() { S = $"ORDER#{order.OrderId}" },
                ["OrderId"] = new() { S = order.OrderId.ToString() },
                ["CustomerId"] = new() { S = order.CustomerId },
                ["Currency"] = new() { S = order.Currency },
                ["ShippingAddress"] = new() { S = order.ShippingAddress },
                ["TotalAmount"] = new() { N = order.TotalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
                ["CreatedAtUtc"] = new() { S = order.CreatedAtUtc.ToString("O") },
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

        await _dynamoDb.PutItemAsync(request, cancellationToken);
    }
}

