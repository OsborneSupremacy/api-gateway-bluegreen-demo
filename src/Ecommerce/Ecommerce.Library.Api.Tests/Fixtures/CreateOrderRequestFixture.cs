using System.Collections.Immutable;
using Bogus;
using Ecommerce.Order.Create.Models;

namespace Ecommerce.Library.Api.Tests.Fixtures;

public sealed class CreateOrderRequestFixture
{
    private readonly Faker _faker = new();
    private readonly Randomizer _randomizer = new();

    public CreateOrderRequest GenerateRandomOrder(int? itemCount = null)
    {
        var count = itemCount ?? _randomizer.Int(1, 10);

        var items = Enumerable.Range(0, count)
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
