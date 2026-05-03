using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using dotenv.net.Utilities;
using Ecommerce.Library.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Library.Extensions;

public static class ServiceProviderBuilderExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddVendorServices()
        {
            var region = RegionEndpoint.GetBySystemName(EnvReader.GetStringValue("AWS_REGION"));
            return services
                .AddDefaultAWSOptions(new AWSOptions { Region = region })
                .AddAWSService<IAmazonDynamoDB>();
        }

        public IServiceCollection AddUtilities() =>
            services
                .AddLogging(builder => builder.AddLambdaLogger())
                .AddSingleton<ApiGatewayAdapter>()
                .AddSingleton<JsonService>();

        public IServiceCollection AddProviders() =>
            services
                .AddSingleton<ApiGatewayAdapter>()
                .AddSingleton<IOrderProvider, OrderProvider>();
    }
}