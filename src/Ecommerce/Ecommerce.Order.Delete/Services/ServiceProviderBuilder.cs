using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Order.Delete.Services;

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