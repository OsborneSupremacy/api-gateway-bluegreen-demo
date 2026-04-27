using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace CreateOrder;

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
                ["OrderId"] = new AttributeValue { S = order.OrderId },
                ["CustomerId"] = new AttributeValue { S = order.CustomerId },
                ["Currency"] = new AttributeValue { S = order.Currency },
                ["ShippingAddress"] = new AttributeValue { S = order.ShippingAddress },
                ["TotalAmount"] = new AttributeValue { N = order.TotalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
                ["CreatedAtUtc"] = new AttributeValue { S = order.CreatedAtUtc.ToString("O") },
                ["Items"] = new AttributeValue
                {
                    L =
                    [
                        .. order.Items.Select(item => new AttributeValue
                        {
                            M = new Dictionary<string, AttributeValue>
                            {
                                ["Sku"] = new AttributeValue { S = item.Sku },
                                ["Name"] = new AttributeValue { S = item.Name },
                                ["Quantity"] = new AttributeValue { N = item.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture) },
                                ["UnitPrice"] = new AttributeValue { N = item.UnitPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
                                ["LineTotal"] = new AttributeValue { N = item.LineTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) }
                            }
                        })
                    ]
                }
            }
        };

        await _dynamoDb.PutItemAsync(request, cancellationToken);
    }
}

