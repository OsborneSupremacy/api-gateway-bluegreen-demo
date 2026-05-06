using System.Diagnostics;
using Ecommerce.Library.Api.Tests.Fixtures;
using FluentAssertions;

namespace Ecommerce.Library.Api.Tests;

public class ApiTests(CreateOrderRequestFixture fixture) : IClassFixture<CreateOrderRequestFixture>
{
    [Fact]
    public void CreateOrder_GivenValidPayload_ReturnsSuccess()
    {
        // arrange
        var request = fixture.GenerateRandomOrder();

        // act

        // write request to console:
        Debug.WriteLine(request);


        // assert
        request.Should().NotBeNull();


    }
}