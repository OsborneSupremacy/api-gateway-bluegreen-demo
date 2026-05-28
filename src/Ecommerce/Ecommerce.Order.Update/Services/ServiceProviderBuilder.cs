using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Order.Update.Services;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddUtilities(
                LibraryJsonTypeInfoResolver.Default,
                OrderUpdateJsonSerializerContext.Default
            )
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
