namespace UrlShortener.Endpoint.Url.Model;

public class CreateShortUrlRequest
{
    public required string OriginalUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? ShortUrlPrefix { get; set; }
}

