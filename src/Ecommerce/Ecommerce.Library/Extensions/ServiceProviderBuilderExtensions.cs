using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using dotenv.net.Utilities;
using Ecommerce.Library.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization.Metadata;

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

        public IServiceCollection AddUtilities(params IJsonTypeInfoResolver?[] jsonTypeInfoResolvers)
        {
            ArgumentNullException.ThrowIfNull(jsonTypeInfoResolvers);

            if (jsonTypeInfoResolvers.Length == 0)
                throw new ArgumentException("At least one JSON type resolver must be provided.", nameof(jsonTypeInfoResolvers));

            services
                .AddLogging(builder => builder.AddLambdaLogger())
                .AddSingleton<ApiGatewayAdapter>();

            foreach (var jsonTypeInfoResolver in jsonTypeInfoResolvers)
            {
                ArgumentNullException.ThrowIfNull(jsonTypeInfoResolver);
                services.AddSingleton<IJsonTypeInfoResolver>(_ => jsonTypeInfoResolver);
            }

            services.AddSingleton<JsonService>();
            return services;
        }

        public IServiceCollection AddProviders() =>
            services
                .AddSingleton<ApiGatewayAdapter>()
                .AddSingleton<IOrderProvider, OrderProvider>();
    }
}