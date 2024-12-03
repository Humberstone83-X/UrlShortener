using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json.Serialization;
using UrlShortener.Endpoint.Url.Model;

namespace UrlShortener.Configuration;

[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(CreateShortUrlRequest))]
[JsonSerializable(typeof(ShortUrlResponse))]
[JsonSerializable(typeof(IHttpResult))]
[JsonSerializable(typeof(string))]
public partial class HttpApiJsonSerializerContext : JsonSerializerContext
{
}
