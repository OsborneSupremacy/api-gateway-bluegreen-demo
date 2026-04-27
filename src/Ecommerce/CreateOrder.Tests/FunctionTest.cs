using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using System.Collections.Immutable;
using System.Text.Json;
using Xunit;

namespace CreateOrder.Tests;

public class FunctionTest
{
    [Fact]
    public async Task FunctionHandler_ReturnsCreatedAndSavesOrder_WhenRequestIsValid()
    {
        var writer = new InMemoryOrderWriter();
        var function = new Function(writer);
        var request = new APIGatewayProxyRequest
        {
            Body = JsonSerializer.Serialize(new CreateOrderRequest
            {
                CustomerId = "customer-123",
                Currency = "usd",
                ShippingAddress = "123 Main St",
                Items = ImmutableList.Create(
                    new CreateOrderItemRequest
                    {
                        Sku = "SKU-001",
                        Name = "Wireless Mouse",
                        Quantity = 2,
                        UnitPrice = 25.50m
                    })
            })
        };

        var response = await function.FunctionHandler(request, new TestLambdaContext());

        Assert.Equal(201, response.StatusCode);
        Assert.Single(writer.Orders);

        var result = JsonSerializer.Deserialize<CreateOrderResult>(response.Body);
        Assert.NotNull(result);
        Assert.Equal("customer-123", result.Order.CustomerId);
        Assert.Equal(51.00m, result.Order.TotalAmount);
    }

    [Fact]
    public async Task FunctionHandler_ReturnsBadRequest_WhenItemsAreMissing()
    {
        var writer = new InMemoryOrderWriter();
        var function = new Function(writer);
        var request = new APIGatewayProxyRequest
        {
            Body = JsonSerializer.Serialize(new CreateOrderRequest
            {
                CustomerId = "customer-456",
                Currency = "USD",
                ShippingAddress = "456 Main St",
                Items = ImmutableList<CreateOrderItemRequest>.Empty
            })
        };

        var response = await function.FunctionHandler(request, new TestLambdaContext());

        Assert.Equal(400, response.StatusCode);
        Assert.Empty(writer.Orders);
    }

    [Fact]
    public async Task FunctionHandler_ReturnsServerError_WhenWriterThrows()
    {
        var function = new Function(new ThrowingOrderWriter());
        var request = new APIGatewayProxyRequest
        {
            Body = JsonSerializer.Serialize(new CreateOrderRequest
            {
                CustomerId = "customer-999",
                Currency = "USD",
                ShippingAddress = "999 Main St",
                Items = ImmutableList.Create(
                    new CreateOrderItemRequest
                    {
                        Sku = "SKU-999",
                        Name = "Mechanical Keyboard",
                        Quantity = 1,
                        UnitPrice = 149.99m
                    })
            })
        };

        var response = await function.FunctionHandler(request, new TestLambdaContext());

        Assert.Equal(500, response.StatusCode);
    }

    private sealed class InMemoryOrderWriter : IOrderWriter
    {
        public List<Order> Orders { get; } = [];

        public Task SaveAsync(Order order, CancellationToken cancellationToken)
        {
            Orders.Add(order);
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingOrderWriter : IOrderWriter
    {
        public Task SaveAsync(Order order, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Simulated persistence failure");
        }
    }
}
