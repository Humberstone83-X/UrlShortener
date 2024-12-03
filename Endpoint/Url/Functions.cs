using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http;
using Serilog;
using UrlShortener.Endpoint.Url.Model;
using UrlShortener.ShortenerService;
using Amazon.Lambda.Serialization.SystemTextJson;
using UrlShortener.Configuration;

[assembly: LambdaGlobalProperties(GenerateMain = true)]
[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<HttpApiJsonSerializerContext>))]

namespace UrlShortener.Endpoint.Url;
public class Functions
{
    internal const string ROUTE = "/Url";

    /// <summary>
    /// Creates a shortened URL.
    /// </summary>
    /// <param name="request">The input model containing the original URL and expiry date.</param>
    /// <returns>The generated short URL.</returns>
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, ROUTE)]
    public async Task<IHttpResult> CreateShortUrl([FromBody] CreateShortUrlRequest request, ILambdaContext context, [FromServices] IUrlShortenerService urlShortenerService)
    { 
        try
        {
            Log.Information("Creating short URL for {OriginalUrl}", request.OriginalUrl);

            var errors = request.ValidateCreateShortUrlRequest();
            if (errors.Count > 0)
            {
                Log.Debug("Validation failed for CreateShortUrlRequest: {Errors}", errors);
                return HttpResults.BadRequest(errors);
            }

            var shortUrl = await urlShortenerService.CreateShortUrlAsync(request.OriginalUrl, request.ExpiryDate, request.ShortUrlPrefix);

            Log.Information("Short URL created: {ShortUrl}", shortUrl.Slug);

            return HttpResults.Created($"{ROUTE}/{shortUrl.Slug}", shortUrl);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create short URL.");
            return HttpResults.InternalServerError("An exception occured.");
        }
    }

    /// <summary>
    /// Retrieves the original URL for a given slug.
    /// </summary>
    /// <param name="slug">The short URL slug.</param>
    /// <returns>The original URL if it exists and is active.</returns>
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, ROUTE + "/{slug}")]
    public async Task<IHttpResult> GetOriginalUrl(string slug, ILambdaContext context, [FromServices] IUrlShortenerService urlShortenerService)
    {
        try
        {
            Log.Information("Retrieving original URL for {Slug}", slug);

            var originalUrl = await urlShortenerService.GetOriginalUrlAsync(slug);

            return originalUrl == null ? HttpResults.NotFound("URL not found or expired.") : HttpResults.Ok(originalUrl);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve original URL.");
            return HttpResults.InternalServerError("An exception occured.");
        }
    }

    /// <summary>
    /// Deletes or deactivates a short URL.
    /// </summary>
    /// <param name="slug">The short URL slug to delete.</param>
    /// <returns>No content if successful.</returns>
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, ROUTE + "/{slug}")]
    public async Task<IHttpResult> DeleteShortUrl(string slug, ILambdaContext context, [FromServices] IUrlShortenerService urlShortenerService)
    {
        try
        {
            Log.Information("Deleting short URL for {Slug}", slug);

            var success = await urlShortenerService.DeleteShortUrlAsync(slug);

            return !success ? HttpResults.NotFound("URL not found.") : HttpResults.Ok();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete short URL.");
            return HttpResults.InternalServerError("An exception occured.");
        }
    }
}
