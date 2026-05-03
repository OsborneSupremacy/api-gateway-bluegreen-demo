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
}
