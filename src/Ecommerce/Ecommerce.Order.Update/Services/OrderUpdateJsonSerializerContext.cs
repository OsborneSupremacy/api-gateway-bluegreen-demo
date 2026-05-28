using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json.Serialization;

namespace Ecommerce.Order.Update.Services;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(UpdateOrderRequest))]
[JsonSerializable(typeof(UpdateOrderItemRequest))]
[JsonSerializable(typeof(ImmutableList<UpdateOrderItemRequest>))]
public partial class OrderUpdateJsonSerializerContext : JsonSerializerContext
{
}