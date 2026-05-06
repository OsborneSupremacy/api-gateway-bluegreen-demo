using System.Collections.Immutable;
using Bogus;
using Ecommerce.Order.Create.Models;

namespace Ecommerce.Library.Api.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ApiTestsFixture
{
    private readonly Faker _faker = new();

    private readonly Randomizer _randomizer = new();

    public CreateOrderRequest GenerateRandomOrder()
    {

        var items = Enumerable.Range(0, _randomizer.Int(1, 10))
            .Select(_ => new CreateOrderItemRequest
            {
                Sku = _faker.Commerce.Ean13(),
                Name = _faker.Commerce.ProductName(),
                Quantity = _randomizer.Int(1, 10),
                UnitPrice = _randomizer.Decimal(1, 100)
            })
            .ToImmutableList();

        return new CreateOrderRequest
        {
            CustomerId = Guid.CreateVersion7().ToString(),
            Currency = "USD",
            ShippingAddress = _faker.Address.FullAddress(),
            Items = items
        };
    }
}
