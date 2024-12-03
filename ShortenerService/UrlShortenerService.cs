using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using NanoidDotNet;
using Serilog;
using System.Net;
using UrlShortener.Configuration;
using UrlShortener.Endpoint.Url.Model;

namespace UrlShortener.ShortenerService;

public class UrlShortenerService(IAmazonDynamoDB client) : IUrlShortenerService
{
    private readonly IAmazonDynamoDB _client = client ?? throw new ArgumentNullException(nameof(client));

    public async Task<bool> DeleteShortUrlAsync(string slug) 
    {
        try
        {
            var request = new DeleteItemRequest
            {
                TableName = AwsParameterStore.AppConfiguration!.ShortenedUrlTable,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "slug", new AttributeValue { S = slug } }
                }
            };

            var response = await _client.DeleteItemAsync(request);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Log.Error($"Error deleting URL with slug {slug}: {ex.Message}");
            throw;
        }
    }

    public async Task<string?> GetOriginalUrlAsync(string slug)
    {
        try
        {
            var getRequest = new GetItemRequest
            {
                TableName = AwsParameterStore.AppConfiguration!.ShortenedUrlTable,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "slug", new AttributeValue { S = slug } }
                }
            };

            var response = await _client.GetItemAsync(getRequest);

            return response.Item == null || response.Item.Count == 0
                ? null
                : response.Item.TryGetValue("original_url", out var value)
                    ? value!.S
                    : null;
        }
        catch (Exception ex)
        {
            Log.Error($"Error getting URL with slug {slug}: {ex.Message}");
            throw;
        }
    }

    public async Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiryDate, string? shortUrlPrefix) 
    {
        try
        {
            var slug = Nanoid.Generate();

            var request = new PutItemRequest
            {
                TableName = AwsParameterStore.AppConfiguration!.ShortenedUrlTable,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "slug", new AttributeValue { S = slug } },
                    { "original_url", new AttributeValue { S = originalUrl } }
                }
            };

            if (expiryDate.HasValue)
            {
                request.Item["expiry_date"] = new AttributeValue { S = expiryDate.Value.ToString("O") };
            }

            var response = await _client.PutItemAsync(request);

            return response.HttpStatusCode == HttpStatusCode.OK
                ? new ShortUrlResponse()
                {
                    Slug = slug,
                    ShortUrl = shortUrlPrefix != null ? $"{shortUrlPrefix}/{slug}" : $"monek.com/{slug}"
                }
                : throw new Exception($"Failed to save URL. Status code: {response.HttpStatusCode}");
        }
        catch (Exception ex)
        {
            Log.Error($"Error creating short URL for {originalUrl}: {ex.Message}");
            throw;
        }
    }
}
