using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Api.Tests.Models;

namespace Ecommerce.Api.Tests;

public class DeleteOrderTests(ApiTestsFixture fixture) : IClassFixture<ApiTestsFixture>
{
    [Fact]
    public async Task DeleteOrder_GivenExistingOrder_ReturnsNoContent()
    {
        // arrange
        var order = await fixture.CreateRandomOrderAsync();
        var httpClient = fixture.GetHttpClient();

        var deleteRequest = new DeleteOrderRequest
        {
            CustomerId = order.CustomerId,
            OrderId = order.OrderId
        };

        // act
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "v1/order")
        {
            Content = JsonContent.Create(deleteRequest)
        });

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrder_GivenNonExistentOrder_ReturnsNoContent()
    {
        // arrange
        var httpClient = fixture.GetHttpClient();

        var deleteRequest = new DeleteOrderRequest
        {
            CustomerId = Guid.CreateVersion7(),
            OrderId = Guid.CreateVersion7()
        };

        // act
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "v1/order")
        {
            Content = JsonContent.Create(deleteRequest)
        });

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
