using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UpdateOrder;

public class Function
{
    private IServiceProvider? _serviceProvider;
    private readonly Lock _serviceProviderLock = new();

    private IServiceProvider GetServiceProvider()
    {
        if (_serviceProvider is not null)
            return _serviceProvider;

        using var lockScope = _serviceProviderLock.EnterScope();
        return _serviceProvider ??= ServiceProviderBuilder.Build();
    }

    [UsedImplicitly]
    public async Task<APIGatewayProxyResponse> FunctionHandler(
        APIGatewayProxyRequest request,
        ILambdaContext _)
        =>
            await GetServiceProvider()
                .GetRequiredService<OrderService>()
                .FunctionHandler(request);
}
