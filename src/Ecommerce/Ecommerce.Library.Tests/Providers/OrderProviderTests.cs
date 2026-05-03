using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Ecommerce.Library.Models;
using Ecommerce.Library.Providers;
using Ecommerce.Library.Tests.Utility;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ecommerce.Library.Tests.Providers;

public sealed class OrderProviderTests
{
    [Fact]
    public async Task CreateAsync_ShouldSendExpectedPutItemRequest()
    {
        // Arrange
        using var envScope = new EnvironmentVariableScope("ORDERS_TABLE_NAME", "orders-table");

        var orderId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var createdAtUtc = DateTimeOffset.Parse("2026-05-01T12:34:56.0000000+00:00", null, System.Globalization.DateTimeStyles.RoundtripKind);

        var order = new Order
        {
            OrderId = orderId,
            CustomerId = "cust-123",
            Currency = "USD",
            ShippingAddress = "123 Test St",
            TotalAmount = 27.48m,
            CreatedAtUtc = createdAtUtc,
            Items =
            [
                new OrderLine
                {
                    Sku = "SKU-1",
                    Name = "First Item",
                    Quantity = 2,
                    UnitPrice = 9.99m,
                    LineTotal = 19.98m
                },
                new OrderLine
                {
                    Sku = "SKU-2",
                    Name = "Second Item",
                    Quantity = 1,
                    UnitPrice = 7.50m,
                    LineTotal = 7.50m
                }
            ]
        };

        PutItemRequest? capturedRequest = null;
        CancellationToken capturedCancellationToken = CancellationToken.None;

        var dynamoDbMock = new Mock<IAmazonDynamoDB>(MockBehavior.Strict);
        dynamoDbMock
            .Setup(client => client.PutItemAsync(It.IsAny<PutItemRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PutItemRequest, CancellationToken>((request, cancellationToken) =>
            {
                capturedRequest = request;
                capturedCancellationToken = cancellationToken;
            })
            .ReturnsAsync(new PutItemResponse());

        var provider = new OrderProvider(dynamoDbMock.Object);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await provider.CreateAsync(order, cancellationToken);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedCancellationToken.Should().Be(cancellationToken);

        capturedRequest!.TableName.Should().Be("orders-table");
        capturedRequest.Item["PK"].S.Should().Be("CUSTOMER#cust-123#ORDER");
        capturedRequest.Item["SK"].S.Should().Be($"ORDER#{orderId}");
        capturedRequest.Item["OrderId"].S.Should().Be(orderId.ToString());
        capturedRequest.Item["CustomerId"].S.Should().Be("cust-123");
        capturedRequest.Item["Currency"].S.Should().Be("USD");
        capturedRequest.Item["ShippingAddress"].S.Should().Be("123 Test St");
        capturedRequest.Item["TotalAmount"].N.Should().Be("27.48");
        capturedRequest.Item["CreatedAtUtc"].S.Should().Be("2026-05-01T12:34:56.0000000+00:00");

        capturedRequest.Item["Items"].L.Should().HaveCount(2);
        capturedRequest.Item["Items"].L[0].M["Sku"].S.Should().Be("SKU-1");
        capturedRequest.Item["Items"].L[0].M["Name"].S.Should().Be("First Item");
        capturedRequest.Item["Items"].L[0].M["Quantity"].N.Should().Be("2");
        capturedRequest.Item["Items"].L[0].M["UnitPrice"].N.Should().Be("9.99");
        capturedRequest.Item["Items"].L[0].M["LineTotal"].N.Should().Be("19.98");

        capturedRequest.Item["Items"].L[1].M["Sku"].S.Should().Be("SKU-2");
        capturedRequest.Item["Items"].L[1].M["Name"].S.Should().Be("Second Item");
        capturedRequest.Item["Items"].L[1].M["Quantity"].N.Should().Be("1");
        capturedRequest.Item["Items"].L[1].M["UnitPrice"].N.Should().Be("7.50");
        capturedRequest.Item["Items"].L[1].M["LineTotal"].N.Should().Be("7.50");

        dynamoDbMock.Verify(client => client.PutItemAsync(It.IsAny<PutItemRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrder_ShouldReturnExpectedOrder_WhenItemExists()
    {
        // Arrange
        using var envScope = new EnvironmentVariableScope("ORDERS_TABLE_NAME", "orders-table");

        var orderId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var capturedRequest = default(GetItemRequest);

        var dynamoDbMock = new Mock<IAmazonDynamoDB>(MockBehavior.Strict);
        dynamoDbMock
            .Setup(client => client.GetItemAsync(It.IsAny<GetItemRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GetItemRequest, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new GetItemResponse
            {
                Item = new Dictionary<string, AttributeValue>
                {
                    ["OrderId"] = new() { S = orderId.ToString() },
                    ["CustomerId"] = new() { S = "cust-123" },
                    ["Currency"] = new() { S = "USD" },
                    ["ShippingAddress"] = new() { S = "123 Test St" },
                    ["TotalAmount"] = new() { N = "27.48" },
                    ["CreatedAtUtc"] = new() { S = "2026-05-01T12:34:56.0000000+00:00" },
                    ["Items"] = new()
                    {
                        L =
                        [
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    ["Sku"] = new() { S = "SKU-1" },
                                    ["Name"] = new() { S = "First Item" },
                                    ["Quantity"] = new() { N = "2" },
                                    ["UnitPrice"] = new() { N = "9.99" },
                                    ["LineTotal"] = new() { N = "19.98" }
                                }
                            }
                        ]
                    }
                }
            });

        var provider = new OrderProvider(dynamoDbMock.Object);

        // Act
        var result = await provider.GetOrder("cust-123", orderId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.OrderId.Should().Be(orderId);
        result.CustomerId.Should().Be("cust-123");
        result.Currency.Should().Be("USD");
        result.ShippingAddress.Should().Be("123 Test St");
        result.TotalAmount.Should().Be(27.48m);
        result.CreatedAtUtc.Should().Be(DateTimeOffset.Parse("2026-05-01T12:34:56.0000000+00:00", null, System.Globalization.DateTimeStyles.RoundtripKind));
        result.Items.Should().HaveCount(1);
        result.Items[0].Sku.Should().Be("SKU-1");

        capturedRequest.Should().NotBeNull();
        capturedRequest!.TableName.Should().Be("orders-table");
        capturedRequest.Key["PK"].S.Should().Be("CUSTOMER#cust-123#ORDER");
        capturedRequest.Key["SK"].S.Should().Be($"ORDER#{orderId}");
    }

    [Fact]
    public async Task UpdateOrder_ShouldSendExpectedUpdateItemRequest()
    {
        // Arrange
        using var envScope = new EnvironmentVariableScope("ORDERS_TABLE_NAME", "orders-table");

        var orderId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var order = new Order
        {
            OrderId = orderId,
            CustomerId = "cust-123",
            Currency = "CAD",
            ShippingAddress = "456 Updated Ave",
            TotalAmount = 100.05m,
            CreatedAtUtc = DateTimeOffset.Parse("2026-05-02T09:10:11.0000000+00:00", null, System.Globalization.DateTimeStyles.RoundtripKind),
            Items =
            [
                new OrderLine
                {
                    Sku = "SKU-3",
                    Name = "Updated Item",
                    Quantity = 5,
                    UnitPrice = 20.01m,
                    LineTotal = 100.05m
                }
            ]
        };

        var capturedRequest = default(UpdateItemRequest);
        var dynamoDbMock = new Mock<IAmazonDynamoDB>(MockBehavior.Strict);
        dynamoDbMock
            .Setup(client => client.UpdateItemAsync(It.IsAny<UpdateItemRequest>(), It.IsAny<CancellationToken>()))
            .Callback<UpdateItemRequest, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new UpdateItemResponse());

        var provider = new OrderProvider(dynamoDbMock.Object);

        // Act
        await provider.UpdateOrder(order, CancellationToken.None);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.TableName.Should().Be("orders-table");
        capturedRequest.Key["PK"].S.Should().Be("CUSTOMER#cust-123#ORDER");
        capturedRequest.Key["SK"].S.Should().Be($"ORDER#{orderId}");
        capturedRequest.ConditionExpression.Should().Be("attribute_exists(PK) AND attribute_exists(SK)");
        capturedRequest.UpdateExpression.Should().Contain("OrderId");
        capturedRequest.ExpressionAttributeValues[":currency"].S.Should().Be("CAD");
        capturedRequest.ExpressionAttributeValues[":shippingAddress"].S.Should().Be("456 Updated Ave");
        capturedRequest.ExpressionAttributeValues[":totalAmount"].N.Should().Be("100.05");
        capturedRequest.ExpressionAttributeValues[":items"].L.Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteOrder_ShouldSendExpectedDeleteItemRequest()
    {
        // Arrange
        using var envScope = new EnvironmentVariableScope("ORDERS_TABLE_NAME", "orders-table");

        var orderId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var capturedRequest = default(DeleteItemRequest);

        var dynamoDbMock = new Mock<IAmazonDynamoDB>(MockBehavior.Strict);
        dynamoDbMock
            .Setup(client => client.DeleteItemAsync(It.IsAny<DeleteItemRequest>(), It.IsAny<CancellationToken>()))
            .Callback<DeleteItemRequest, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new DeleteItemResponse());

        var provider = new OrderProvider(dynamoDbMock.Object);

        // Act
        await provider.DeleteOrder("cust-123", orderId, CancellationToken.None);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.TableName.Should().Be("orders-table");
        capturedRequest.Key["PK"].S.Should().Be("CUSTOMER#cust-123#ORDER");
        capturedRequest.Key["SK"].S.Should().Be($"ORDER#{orderId}");
    }
}
