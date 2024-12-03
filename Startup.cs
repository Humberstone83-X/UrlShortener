using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using UrlShortener.Configuration;
using UrlShortener.ShortenerService;

namespace UrlShortener;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var logLevel = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Information";
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Is(ParseLogLevel(logLevel))
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders(); 
            loggingBuilder.AddSerilog(dispose: true);
        });

        services.AddAWSLambdaHosting(LambdaEventSource.HttpApi, new SourceGeneratorLambdaJsonSerializer<HttpApiJsonSerializerContext>());

        AwsParameterStore.GetValueAsync("/service/urlshortener/config").GetAwaiter().GetResult();

        services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
        services.AddScoped<IUrlShortenerService, UrlShortenerService>();

        services.AddEndpointsApiExplorer();

        services.AddCors(options =>
        {
            options.AddPolicy("app-policy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    static LogEventLevel ParseLogLevel(string level)
    {
        return level.ToLower() switch
        {
            "debug" => LogEventLevel.Debug,
            "information" => LogEventLevel.Information,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}
