using Ecommerce.Api.Tests.Messaging;
using Ecommerce.Api.Tests.Models;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Api.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ApiTestsFixture : IDisposable
{
    private readonly Faker _faker = new();

    private readonly Randomizer _randomizer = new();

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    private HttpClient? _httpClient;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    private readonly Lock _httpClientLock = new();

    public HttpClient GetHttpClient()
    {
        if(_httpClient is not null)
            return _httpClient;
        using var lockScope = _httpClientLock.EnterScope();

#if DEBUG
        DotEnv.Load();
#endif
        var baseAddress = new Uri(EnvReader.GetStringValue("BASE_ADDRESS"));

        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; ApiTests/1.0)");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetApiToken()}");
        client.BaseAddress = baseAddress;
        return _httpClient ??= client;
    }

    private string GetApiToken()
    {
        // ReSharper disable once RedundantAssignment
        var apiToken = string.Empty;
#if DEBUG
        // if running locally, try to get API token from user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ApiTestsFixture>(optional: true)
            .Build();
        apiToken = configuration["API_TOKEN"];
#endif
        // ReSharper disable once InvertIf
        if (string.IsNullOrEmpty(apiToken))
        {
#if DEBUG
            DotEnv.Load();
#endif
            EnvReader.TryGetStringValue("API_TOKEN", out apiToken);
        }

        return string.IsNullOrWhiteSpace(apiToken)
            ? throw new Exception("API_TOKEN is not set in environment variables, .env, or user secrets.")
            : apiToken;
    }

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
            CustomerId = Guid.CreateVersion7(),
            Currency = "USD",
            ShippingAddress = _faker.Address.FullAddress(),
            Items = items
        };
    }

    public async Task<OrderMetaData> CreateRandomOrderAsync()
    {
        var request = GenerateRandomOrder();
        var client = GetHttpClient();
        var response =  await client.PostAsJsonAsync("v1/order", request);

        if(response.StatusCode != HttpStatusCode.Created)
            throw new AggregateException("Failed to create order");

        var responseBody = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        return new OrderMetaData
        {
            CustomerId = request.CustomerId,
            OrderId = responseBody!.OrderId
        };
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
