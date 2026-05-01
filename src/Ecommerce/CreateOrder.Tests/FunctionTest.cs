using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using System.Collections.Immutable;
using System.Text.Json;
using CreateOrder.Abstractions;
using CreateOrder.Models;
using CreateOrder.Validators;
using Ecommerce.Library.Models;
using Xunit;

namespace CreateOrder.Tests;

public class FunctionTest
{
    [Fact]
    public async Task FunctionHandler_ReturnsCreatedAndSavesOrder_WhenRequestIsValid()
    {
        var writer = new InMemoryOrderProvider();
        var function = new Function(writer, new CreateOrderRequestValidator());
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

        var result = JsonSerializer.Deserialize<CreateOrderResponse>(response.Body);
        Assert.NotNull(result?.OrderId);
    }

    [Fact]
    public async Task FunctionHandler_ReturnsBadRequest_WhenItemQuantityIsZero()
    {
        var writer = new InMemoryOrderProvider();
        var function = new Function(writer, new CreateOrderRequestValidator());
        var request = new APIGatewayProxyRequest
        {
            Body = JsonSerializer.Serialize(new CreateOrderRequest
            {
                CustomerId = "customer-456",
                Currency = "USD",
                ShippingAddress = "456 Main St",
                Items = ImmutableList.Create(new CreateOrderItemRequest
                {
                    Sku = "SKU-001",
                    Name = "Wireless Mouse",
                    Quantity = 0,
                    UnitPrice = 25.50m
                })
            })
        };

        var response = await function.FunctionHandler(request, new TestLambdaContext());

        Assert.Equal(400, response.StatusCode);
        Assert.Empty(writer.Orders);
    }

    [Fact]
    public async Task FunctionHandler_ReturnsBadRequest_WhenItemUnitPriceIsZero()
    {
        var writer = new InMemoryOrderProvider();
        var function = new Function(writer, new CreateOrderRequestValidator());
        var request = new APIGatewayProxyRequest
        {
            Body = JsonSerializer.Serialize(new CreateOrderRequest
            {
                CustomerId = "customer-456",
                Currency = "USD",
                ShippingAddress = "456 Main St",
                Items = ImmutableList.Create(new CreateOrderItemRequest
                {
                    Sku = "SKU-001",
                    Name = "Wireless Mouse",
                    Quantity = 1,
                    UnitPrice = 0m
                })
            })
        };

        var response = await function.FunctionHandler(request, new TestLambdaContext());

        Assert.Equal(400, response.StatusCode);
        Assert.Empty(writer.Orders);
    }

    [Fact]
    public async Task FunctionHandler_ReturnsServerError_WhenWriterThrows()
    {
        var function = new Function(new ThrowingOrderProvider(), new CreateOrderRequestValidator());
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

    private sealed class InMemoryOrderProvider : IOrderProvider
    {
        public List<Order> Orders { get; } = [];

        public Task SaveAsync(Order order, CancellationToken cancellationToken)
        {
            Orders.Add(order);
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingOrderProvider : IOrderProvider
    {
        public Task SaveAsync(Order order, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Simulated persistence failure");
        }
    }
}
