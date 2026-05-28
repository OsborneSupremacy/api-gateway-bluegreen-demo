using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json.Serialization;

namespace Ecommerce.Order.Delete.Services;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(DeleteOrderRequest))]
public partial class OrderDeleteJsonSerializerContext : JsonSerializerContext
{
}