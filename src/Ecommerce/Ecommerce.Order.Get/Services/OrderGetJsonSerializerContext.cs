using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json.Serialization;

namespace Ecommerce.Order.Get.Services;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(GetOrderRequest))]
[JsonSerializable(typeof(GetOrderResponse))]
[JsonSerializable(typeof(GetOrderItemResponse))]
[JsonSerializable(typeof(ImmutableList<GetOrderItemResponse>))]
public partial class OrderGetJsonSerializerContext : JsonSerializerContext
{
}