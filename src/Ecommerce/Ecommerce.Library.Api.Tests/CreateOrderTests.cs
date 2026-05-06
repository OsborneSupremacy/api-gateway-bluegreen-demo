namespace Ecommerce.Library.Api.Tests;

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
        var responseBody = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        responseBody.Should().NotBeNull();
        responseBody.OrderId.Should().NotBe(Guid.Empty);
    }
}