using Ecommerce.Api.Tests.Fixtures;

namespace Ecommerce.Api.Tests;

public class UpdateOrderTests(ApiTestsFixture fixture) : IClassFixture<ApiTestsFixture>
{
    private readonly Faker _faker = new();

    private readonly Randomizer _randomizer = new();

    [Fact]
    public async Task UpdateOrder_GivenValidPayload_ReturnsSuccess()
    {
        // arrange
        var order = await fixture.CreateRandomOrderAsync();
        var httpClient = fixture.GetHttpClient();

        var updateRequest = new UpdateOrderRequest
        {
            CustomerId = order.CustomerId,
            OrderId = order.OrderId,
            Currency = "USD",
            ShippingAddress = _faker.Address.FullAddress(),
            Items =
            [
                new UpdateOrderItemRequest
                {
                    Sku = _faker.Commerce.Ean13(),
                    Name = _faker.Commerce.ProductName(),
                    Quantity = _randomizer.Int(1, 10),
                    UnitPrice = _randomizer.Decimal(1, 100)
                }
            ]
        };

        // act
        var response = await httpClient.PutAsJsonAsync("v1/order", updateRequest);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrder_GivenNonExistentOrder_ReturnsNotFound()
    {
        // arrange
        var httpClient = fixture.GetHttpClient();

        var updateRequest = new UpdateOrderRequest
        {
            CustomerId = Guid.CreateVersion7().ToString(),
            OrderId = Guid.NewGuid(),
            Currency = "USD",
            ShippingAddress = _faker.Address.FullAddress(),
            Items =
            [
                new UpdateOrderItemRequest
                {
                    Sku = _faker.Commerce.Ean13(),
                    Name = _faker.Commerce.ProductName(),
                    Quantity = _randomizer.Int(1, 10),
                    UnitPrice = _randomizer.Decimal(1, 100)
                }
            ]
        };

        // act
        var response = await httpClient.PutAsJsonAsync("v1/order", updateRequest);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
