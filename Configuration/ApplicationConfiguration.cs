namespace UrlShortener.Configuration;

public record ApplicationConfiguration
{
    public required string ShortenedUrlTable { get; init; }
}
