using System.Text.Json.Serialization.Metadata;

namespace Ecommerce.Library.Services;

public class JsonService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonService(IJsonTypeInfoResolver jsonTypeInfoResolver)
    {
        ArgumentNullException.ThrowIfNull(jsonTypeInfoResolver);

        _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = jsonTypeInfoResolver
        };
    }

    public string SerializeDefault<T>(T value) =>
        JsonSerializer.Serialize(value, _jsonSerializerOptions);

    public T? DeserializeDefault<T>(string value) =>
        JsonSerializer.Deserialize<T>(value, _jsonSerializerOptions);
}
