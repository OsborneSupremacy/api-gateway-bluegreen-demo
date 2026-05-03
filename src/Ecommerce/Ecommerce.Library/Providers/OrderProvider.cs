using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using dotenv.net.Utilities;
using Ecommerce.Library.Abstractions;
using Ecommerce.Library.Models;

namespace Ecommerce.Library.Providers;

public sealed class OrderProvider : IOrderProvider
{
    private readonly IAmazonDynamoDB _dynamoDbClient;

    private readonly string _tableName;

    public OrderProvider(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDbClient = dynamoDb ?? throw new ArgumentNullException(nameof(dynamoDb));
        _tableName = EnvReader.GetStringValue("ORDERS_TABLE_NAME");
    }

    public async Task CreateAsync(Order order, CancellationToken cancellationToken)
    {
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

        await _dynamoDbClient
            .PutItemAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}