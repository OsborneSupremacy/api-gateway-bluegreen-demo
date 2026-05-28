using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization.Metadata;
using Ecommerce.Library.Services;

namespace Ecommerce.Order.Delete.Services;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddUtilities(
                JsonTypeInfoResolver.Combine(
                    LibraryJsonTypeInfoResolver.Default,
                    OrderDeleteJsonSerializerContext.Default
                )
            )
            .AddVendorServices()
            .AddProviders()
            .AddBusinessServices()
            .BuildServiceProvider();

    extension(IServiceCollection services)
    {
        // ReSharper disable once MemberCanBePrivate.Global
        internal IServiceCollection AddBusinessServices() =>
            services
                .AddSingleton<AbstractValidator<DeleteOrderRequest>, DeleteOrderRequestValidator>()
                .AddSingleton<OrderService>();
    }
}