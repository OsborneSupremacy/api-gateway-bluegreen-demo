using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateOrder;

public class Function
{
    private IServiceProvider? _serviceProvider;
    private readonly Lock _serviceProviderLock = new();

    private IServiceProvider GetServiceProvider()
    {
        if(_serviceProvider is not null)
            return _serviceProvider;
        using var lockScope = _serviceProviderLock.EnterScope();
        return _serviceProvider ??= ServiceProviderBuilder.Build();
    }

    [UsedImplicitly]
    public async Task<APIGatewayProxyResponse> FunctionHandler(
        APIGatewayProxyRequest request,
        ILambdaContext _
        ) =>
        await GetServiceProvider()
            .GetRequiredService<OrderService>()
            .FunctionHandler(request);
}