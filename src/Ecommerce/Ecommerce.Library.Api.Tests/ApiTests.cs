using System.Collections.Immutable;
using System.Diagnostics;
using Ecommerce.Order.Create.Models;
using FluentAssertions;

namespace Ecommerce.Library.Api.Tests;

public class ApiTests
{
    [Fact]
    public void CreateOrder_GivenValidPayload_ReturnsSuccess()
    {
        // arrange
        var itemCount = new Bogus.Randomizer().Int(1, 10);

        var items = new List<CreateOrderItemRequest>();

        for (var i = 0; i < itemCount; i++)
            items.Add(new CreateOrderItemRequest
            {
                Sku = new Bogus.Faker().Commerce.Ean13(),
                Name = new Bogus.Faker().Commerce.ProductName(),
                Quantity = new Bogus.Randomizer().Int(1, 10),
                UnitPrice = new Bogus.Randomizer().Decimal(1, 100)
            });

        var request = new CreateOrderRequest
        {
            CustomerId = Guid.CreateVersion7().ToString(),
            Currency = "USD",
            ShippingAddress = new Bogus.Faker().Address.FullAddress(),
            Items = items.ToImmutableList()
        };

        // act

        // write request to console:
        Debug.WriteLine(request);


        // assert
        request.Should().NotBeNull();


    }
}