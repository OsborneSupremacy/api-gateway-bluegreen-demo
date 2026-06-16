using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Order.Delete;

public class Function
{
    private readonly OrderService _orderService;

    internal Function(OrderService orderService) =>
        _orderService = orderService;

    public static async Task Main()
    {
        var provider = ServiceProviderBuilder.Build();
        var function = new Function(provider.GetRequiredService<OrderService>());
        var handler = function.FunctionHandler;

        await LambdaBootstrapBuilder
            .Create(handler, new SourceGeneratorLambdaJsonSerializer<OrderDeleteJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

    [UsedImplicitly]
    public Task<APIGatewayProxyResponse> FunctionHandler(
        APIGatewayProxyRequest request,
        ILambdaContext _)
        =>
            _orderService.FunctionHandler(request);
}
