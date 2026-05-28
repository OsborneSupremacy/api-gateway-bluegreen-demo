using Amazon.Lambda.APIGatewayEvents;

namespace Ecommerce.Library.Builders;

/// <summary>
/// Provides methods to build APIGatewayProxyResponse objects with appropriate status codes, headers, and optional body content.
/// </summary>
public static class ProxyResponseBuilder
{
    public static APIGatewayProxyResponse Build(HttpStatusCode statusCode) =>
        new()
        {
            StatusCode = (int)statusCode,
            Headers = CorsHeaderProvider.GetCorsHeaders()
        };

    public static APIGatewayProxyResponse Build(HttpStatusCode statusCode, string body) =>
        new()
        {
            StatusCode = (int)statusCode,
            Body = body,
            Headers = CorsHeaderProvider.GetCorsHeaders()
        };
}
