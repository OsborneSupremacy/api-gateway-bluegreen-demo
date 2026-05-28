using Amazon.Lambda.APIGatewayEvents;

namespace Ecommerce.Library.Abstractions;

/// <summary>
/// Defines a contract for handling API Gateway requests in AWS Lambda functions.
/// Implementing classes must provide a method to process incoming API Gateway requests and return appropriate responses.
/// This interface promotes a consistent approach to handling API Gateway events across different services within the e-commerce application.
/// </summary>
public interface IApiGatewayHandler
{
    /// <summary>
    /// Handles an incoming API Gateway request and returns a response.
    /// </summary>
    /// <param name="request">The API Gateway request.</param>
    /// <returns>The API Gateway response.</returns>
    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request);
}
