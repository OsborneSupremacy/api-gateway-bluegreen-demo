using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using JetBrains.Annotations;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Ecommerce.Authorizer;

public class Function
{
    private const string TokenEnvironmentVariable = "API_TOKEN";

    [UsedImplicitly]
    public static APIGatewayCustomAuthorizerResponse FunctionHandler(
        APIGatewayCustomAuthorizerRequest request,
        ILambdaContext _
        )
    {
        var expectedToken = Environment.GetEnvironmentVariable(TokenEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(expectedToken))
            throw new Exception("Unauthorized");

        var providedToken = NormalizeBearerToken(request.AuthorizationToken);

        return !string.Equals(providedToken, expectedToken, StringComparison.Ordinal)
            ? throw new Exception("Unauthorized")
            : BuildAllowPolicy("demo-user", request.MethodArn);
    }

    private static string NormalizeBearerToken(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
            return string.Empty;

        const string bearerPrefix = "Bearer ";
        return authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
            ? authorizationHeader[bearerPrefix.Length..].Trim()
            : authorizationHeader.Trim();
    }

    private static APIGatewayCustomAuthorizerResponse BuildAllowPolicy(string principalId, string methodArn) =>
        new()
        {
            PrincipalID = principalId,
            PolicyDocument = new APIGatewayCustomAuthorizerPolicy
            {
                Version = "2012-10-17",
                Statement =
                [
                    new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
                    {
                        Action =
                        [
                            "execute-api:Invoke"
                        ],
                        Effect = "Allow",
                        Resource =
                        [
                            methodArn
                        ]
                    }
                ]
            }
        };
}