namespace UrlShortener.Endpoint.Url.Model;

public class ShortUrlResponse
{
    public required string Slug { get; set; }
    public required string ShortUrl { get; set; }
}
