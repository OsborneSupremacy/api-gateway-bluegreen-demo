using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json.Serialization;

namespace Ecommerce.Order.Create.Services;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(CreateOrderRequest))]
[JsonSerializable(typeof(CreateOrderItemRequest))]
[JsonSerializable(typeof(ImmutableList<CreateOrderItemRequest>))]
[JsonSerializable(typeof(CreateOrderResponse))]
public partial class OrderCreateJsonSerializerContext : JsonSerializerContext
{
}