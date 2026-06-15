using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Order.Update;

public class Function
{
    private IServiceProvider? _serviceProvider;
    private readonly Lock _serviceProviderLock = new();

    public static async Task Main()
    {
        var function = new Function();
        var handler = function.FunctionHandler;

        await LambdaBootstrapBuilder
            .Create(handler, new SourceGeneratorLambdaJsonSerializer<OrderUpdateJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

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
