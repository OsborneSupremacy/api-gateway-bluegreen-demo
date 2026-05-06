using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Order.Get.Services;

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
                .AddSingleton<AbstractValidator<GetOrderRequest>, GetOrderRequestValidator>()
                .AddSingleton<OrderService>();
    }
}