using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Order.Update.Services;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddJsonTypeResolver(LibraryJsonTypeInfoResolver.Default)
            .AddJsonTypeResolver(OrderUpdateJsonSerializerContext.Default)
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
