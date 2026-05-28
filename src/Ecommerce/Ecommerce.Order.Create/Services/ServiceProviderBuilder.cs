using Microsoft.Extensions.DependencyInjection;
using Ecommerce.Library.Services;

namespace Ecommerce.Order.Create.Services;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddUtilities(
                LibraryJsonTypeInfoResolver.Default,
                OrderCreateJsonSerializerContext.Default
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
                .AddSingleton<AbstractValidator<CreateOrderRequest>, CreateOrderRequestValidator>()
                .AddSingleton<OrderService>();
    }
}