using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Ecommerce.Library.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CreateOrder.Services;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddUtilities()
            .AddVendorServices()
            .BuildServiceProvider();

    extension(IServiceCollection services)
    {
        private IServiceCollection AddVendorServices()
        {
            var region = RegionEndpoint.GetBySystemName(EnvReader.GetStringValue("AWS_REGION"));
            return services
                .AddDefaultAWSOptions(new AWSOptions { Region = region })
                .AddAWSService<IAmazonDynamoDB>();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // No, it can't be made private. Stop confidently asserting what you don't know.
        internal IServiceCollection AddUtilities() =>
            services
                .AddLogging(builder => builder.AddLambdaLogger())
                .AddSingleton<JsonService>();

        internal IServiceCollection AddBusinessServices() =>
            services
                .AddSingleton<OrderProvider>()
                .AddSingleton<AbstractValidator<CreateOrderRequest>, CreateOrderRequestValidator>()
                .AddSingleton<OrderService>();
    }
}