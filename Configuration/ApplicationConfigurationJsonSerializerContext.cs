using System.Text.Json.Serialization;

namespace UrlShortener.Configuration;

[JsonSerializable(typeof(ApplicationConfiguration))]
public partial class ApplicationConfigurationJsonSerializerContext : JsonSerializerContext
{
}
