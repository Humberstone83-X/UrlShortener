using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Serilog;
using System.Text.Json;

namespace UrlShortener.Configuration;

public static class AwsParameterStore
{
    public static ApplicationConfiguration? AppConfiguration { get; set; }

    public static async Task GetValueAsync(string parameter)
    {
        var ssmClient = new AmazonSimpleSystemsManagementClient(RegionEndpoint.EUWest2);

        try
        {
            var response = await ssmClient.GetParameterAsync(new GetParameterRequest
            {
                Name = parameter,
                WithDecryption = true
            });

            if (string.IsNullOrWhiteSpace(response.Parameter.Value))
            {
                throw new ApplicationException("Unable to load application configuration");
            }

            AppConfiguration = JsonSerializer.Deserialize(
                response.Parameter.Value,
                ApplicationConfigurationJsonSerializerContext.Default.ApplicationConfiguration)!;

            Log.Information("Application configuration: {@parameter} has been loaded.", parameter);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error when loading the application configuration, {ex.Message}");
        }
    }
}
