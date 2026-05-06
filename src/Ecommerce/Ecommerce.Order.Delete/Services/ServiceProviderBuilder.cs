using Ecommerce.Library.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DeleteOrder.Services;

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
        // ReSharper disable once MemberCanBePrivate.Global
        internal IServiceCollection AddBusinessServices() =>
            services
                .AddSingleton<AbstractValidator<DeleteOrderRequest>, DeleteOrderRequestValidator>()
                .AddSingleton<OrderService>();
    }
}