using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using AverbacaoService.startupInfra.Extensions;
using AverbacaoService.startupInfra.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var serviceVersion = Environment.GetEnvironmentVariable("DD_VERSION") ??
                     Assembly.GetExecutingAssembly().GetName().Version?.ToString();

var assemblyName = Assembly.GetExecutingAssembly().GetName();
var serviceName = assemblyName.Name;
try
{
    Console.WriteLine("Starting application");
    Log.ForContext("ApplicationName", serviceName).Information("Starting application");

    var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services
            .AddOptions<KafkaConfig>()
            .Bind(configuration.GetSection("Kafka"))
            .Validate(kafkaConfig => !string.IsNullOrWhiteSpace(kafkaConfig.BootstrapServers), "Kafka BootstrapServers cannot be null or empty.")
            .Validate(kafkaConfig => kafkaConfig.Retry.Attempts > 0, "Retry attempts must be greater than 0.")
            .ValidateOnStart();

        services
            .AddKafka(configuration);
    });
    
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    builder.AddSerilog(configuration);

    var host = builder.Build();

    host.Run();

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("Error when trying to start application {0}", ex);
    var errorContext = new
    {
        ApplicationName = serviceName,
        Version = serviceVersion,
        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
        PodName = Environment.GetEnvironmentVariable("HOSTNAME") ?? "Unknown",
        NodeName = Environment.GetEnvironmentVariable("NODE_NAME") ?? "Unknown",
        Namespace = Environment.GetEnvironmentVariable("POD_NAMESPACE") ?? "Unknown"
    };

    Log.ForContext("ErrorContext", errorContext)
        .Fatal(ex, "Application terminated unexpectedly. Waiting for logs to be sent...");

    Thread.Sleep(10000);

    return 1;
}
finally
{
    Log.CloseAndFlush();
}

