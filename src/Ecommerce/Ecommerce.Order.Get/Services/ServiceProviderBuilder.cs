using Microsoft.Extensions.DependencyInjection;
using Ecommerce.Library.Services;

namespace Ecommerce.Order.Get.Services;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddJsonOptions([LibraryJsonTypeInfoResolver.Default, OrderGetJsonSerializerContext.Default])
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