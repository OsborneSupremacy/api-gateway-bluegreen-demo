using Ecommerce.Library.Models;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Ecommerce.Library.Services;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(OrderLine))]
[JsonSerializable(typeof(ImmutableList<OrderLine>))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(StatusCodeOnlyResponse))]
internal partial class LibraryJsonSerializerContext : JsonSerializerContext
{
}

public static class LibraryJsonTypeInfoResolver
{
    public static IJsonTypeInfoResolver Default => LibraryJsonSerializerContext.Default;
}