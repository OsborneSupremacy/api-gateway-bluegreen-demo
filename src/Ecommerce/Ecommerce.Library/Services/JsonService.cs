using System.Text.Json.Serialization.Metadata;

namespace Ecommerce.Library.Services;

public class JsonService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonService(IEnumerable<IJsonTypeInfoResolver> jsonTypeInfoResolvers)
    {
        ArgumentNullException.ThrowIfNull(jsonTypeInfoResolvers);

        var resolvers = jsonTypeInfoResolvers.ToArray();

        if (resolvers.Length == 0)
            throw new InvalidOperationException("At least one IJsonTypeInfoResolver must be registered.");

        _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = JsonTypeInfoResolver.Combine(resolvers)
        };
    }

    public string SerializeDefault<T>(T value) =>
        JsonSerializer.Serialize(value, _jsonSerializerOptions);

    public T? DeserializeDefault<T>(string value) =>
        JsonSerializer.Deserialize<T>(value, _jsonSerializerOptions);
}
