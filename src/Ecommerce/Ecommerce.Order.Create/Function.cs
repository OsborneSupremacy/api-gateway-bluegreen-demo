using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Order.Create;

public class Function
{
    private IServiceProvider? _serviceProvider;
    private readonly Lock _serviceProviderLock = new();

    public static async Task Main()
    {
        var function = new Function();
        Func<APIGatewayProxyRequest, ILambdaContext, Task<APIGatewayProxyResponse>> handler = function.FunctionHandler;

        await LambdaBootstrapBuilder
            .Create(handler, new SourceGeneratorLambdaJsonSerializer<OrderCreateJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

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