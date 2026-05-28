using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization.Metadata;
using Ecommerce.Library.Services;

namespace Ecommerce.Order.Get.Services;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddUtilities(
                JsonTypeInfoResolver.Combine(
                    LibraryJsonTypeInfoResolver.Default,
                    OrderGetJsonSerializerContext.Default
                )
            )
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