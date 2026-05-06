using Ecommerce.Api.Tests.Fixtures;

namespace Ecommerce.Api.Tests;

public class GetOrderTests(ApiTestsFixture fixture) : IClassFixture<ApiTestsFixture>
{
    [Fact]
    public async Task GetOrder_GivenExistingOrder_ReturnsSuccess()
    {
        // arrange
        var order = await fixture.CreateRandomOrderAsync();
        var httpClient = fixture.GetHttpClient();

        // act
        var response = await httpClient.GetAsync($"v1/order/{order.CustomerId}/{order.OrderId}");
        var responseBody = await response.Content.ReadFromJsonAsync<GetOrderResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseBody.Should().NotBeNull();
        responseBody.OrderId.Should().Be(order.OrderId);
        responseBody.CustomerId.Should().Be(order.CustomerId);
    }

    [Fact]
    public async Task GetOrder_GivenNonExistentOrder_ReturnsNotFound()
    {
        // arrange
        var httpClient = fixture.GetHttpClient();
        var customerId = Guid.CreateVersion7().ToString();
        var orderId = Guid.NewGuid();

        // act
        var response = await httpClient.GetAsync($"v1/order/{customerId}/{orderId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
