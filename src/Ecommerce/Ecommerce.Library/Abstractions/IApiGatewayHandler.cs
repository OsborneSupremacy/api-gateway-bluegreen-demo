using Amazon.Lambda.APIGatewayEvents;

namespace Ecommerce.Library.Abstractions;

public interface IApiGatewayHandler
{
    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request);
}
