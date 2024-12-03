using UrlShortener.Endpoint.Url.Model;

namespace UrlShortener.Endpoint.Url;

public static class UrlRequestValidator
{
    public static Dictionary<string, string[]> ValidateCreateShortUrlRequest(this CreateShortUrlRequest request)
    {
        Dictionary<string, string[]> errors = [];

        if (string.IsNullOrWhiteSpace(request.OriginalUrl))
        {
            errors.Add(nameof(request.OriginalUrl), ["OriginalUrl is required"]);
        }
        else if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out _))
        {
            errors.Add(nameof(request.OriginalUrl), ["OriginalUrl is not a valid URL"]);
        }

        if( request.ShortUrlPrefix != null && !Uri.TryCreate(request.ShortUrlPrefix, UriKind.RelativeOrAbsolute, out _))
        {
            errors.Add(nameof(request.ShortUrlPrefix), ["ShortUrlPrefix is not a valid URL"]);
        }

        if (request.ExpiryDate != null && request.ExpiryDate < DateTime.UtcNow)
        {
            errors.Add(nameof(request.ExpiryDate), ["Expiry date must be in the future."]);
        }

        return errors;
    }
}
