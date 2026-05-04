using Ecommerce.Library.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace UpdateOrder.Services;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddUtilities()
            .AddVendorServices()
            .AddProviders()
            .AddBusinessServices()
            .BuildServiceProvider();

    extension(IServiceCollection services)
    {
        internal IServiceCollection AddBusinessServices() =>
            services
                .AddSingleton<AbstractValidator<UpdateOrderRequest>, UpdateOrderRequestValidator>()
                .AddSingleton<OrderService>();
    }
}
