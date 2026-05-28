using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json.Serialization;

namespace Ecommerce.Authorizer;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(APIGatewayCustomAuthorizerRequest))]
[JsonSerializable(typeof(APIGatewayCustomAuthorizerResponse))]
[JsonSerializable(typeof(APIGatewayCustomAuthorizerPolicy))]
[JsonSerializable(typeof(APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement))]
public partial class AuthorizerJsonSerializerContext : JsonSerializerContext
{
}