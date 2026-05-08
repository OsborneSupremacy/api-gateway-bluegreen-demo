using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Api.Tests.Models;

namespace Ecommerce.Api.Tests;

public class CreateOrderTests(ApiTestsFixture fixture) : IClassFixture<ApiTestsFixture>
{
    [Fact]
    public async Task CreateOrder_GivenValidPayload_ReturnsSuccess()
    {
        // arrange
        var request = fixture.GenerateRandomOrder();
        var httpClient = fixture.GetHttpClient();

        // act
        var response = await httpClient.PostAsJsonAsync("v1/order", request);
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        responseBody.Should().NotBeNull();
        responseBody.OrderId.Should().NotBe(Guid.Empty);
    }
}