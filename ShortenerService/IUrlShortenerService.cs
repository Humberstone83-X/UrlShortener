
using UrlShortener.Endpoint.Url.Model;

namespace UrlShortener.ShortenerService;

public interface IUrlShortenerService
{
    Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiryDate, string? shortUrlPrefix);
    Task<bool> DeleteShortUrlAsync(string slug);
    Task<string?> GetOriginalUrlAsync(string slug);
}
