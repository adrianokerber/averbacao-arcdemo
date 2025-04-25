using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Averbacao.startupInfra.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Averbacao.startupInfra.Extensions;

internal static class ServicesExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = configuration.GetSection("Kafka").Get<KafkaConfig>();
        if (kafkaConfig == null)
            throw new InvalidOperationException("Kafka configuration is invalid.");

        services.AddSingleton(kafkaConfig);

        services
            .AddSilverback()
            .WithConnectionToMessageBroker(options => options.AddKafka())
            .AddEndpointsConfigurator<KafkaEndpointsConfigurator>()
            .AddScopedSubscriber<RecepcionarItencaoAverbacaoManualEventConsumer>();

        services.AddHealthChecks().AddCheck<KafkaHealthCheck>("Kafka");

        return services;
    }


    public static void AddSerilog(this IHostBuilder builder, IConfiguration configuration)
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);

        var applicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Application";

        builder.UseSerilog((ctx, lc) =>
        {
            lc.Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ApplicationName", $"{applicationName}")
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .MinimumLevel.Is(BuscarNivelLog(configuration))
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch(BuscarNivelLog(configuration)))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

            if (IsElasticEnabled(ctx.Configuration))
                ConfigureElasticsearchLogging(ctx.Configuration, lc);
        });
    }

    private static LogEventLevel BuscarNivelLog(IConfiguration configuration)
    {
        var nivel = configuration["ElasticConfiguration:MinimumLevel"]?.ToUpper();

        return nivel switch
        {
            "VERBOSE" => LogEventLevel.Verbose,
            "DEBUG" => LogEventLevel.Debug,
            "INFORMATION" => LogEventLevel.Information,
            "WARNING" => LogEventLevel.Warning,
            "ERROR" => LogEventLevel.Error,
            "FATAL" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information,
        };
    }

    private static bool IsElasticEnabled(IConfiguration configuration) =>
        bool.TryParse(configuration["ElasticConfiguration:IsEnabledElasticLogs"], out var enabled) && enabled;

    private static void ConfigureElasticsearchLogging(IConfiguration configuration, LoggerConfiguration lc)
    {
        var configurations = ElasticConfiguration.Obter(configuration);

        Action<ElasticsearchSinkOptions> elasticOptions = (opts) =>
        {
            opts.DataStream = new ElasticDataStreamName("logs", configurations.NameSpace, configurations.Environment);
            opts.TextFormatting = new EcsTextFormatterConfiguration();
            opts.BootstrapMethod = BootstrapMethod.Failure;
            opts.IlmPolicy = configurations.IlmPolicy;
        };

        Action<TransportConfigurationDescriptor> elasticTransport = (transport) =>
        {
            transport.Authentication(new BasicAuthentication(configurations.ElasticUsername, configurations.ElasticPassword));

            if (configurations.VerifyCertificate)
            {
                var certificate = GetTrustedCertificate(configurations);

                transport
                    .ClientCertificate(certificate)
                    .ServerCertificateValidationCallback((_, cert, _, _) =>
                        new X509Certificate2(cert).Thumbprint == certificate.Thumbprint);
            }
        };
        lc.WriteTo.Elasticsearch([.. configurations.Uris], elasticOptions, elasticTransport);
    }

    private static X509Certificate2 GetTrustedCertificate(ElasticConfiguration configuration)
    {
        var certPath = configuration.IsDeveloperMode
            ? GetProjectCertPath()
            : configuration.CertificatePath;

        return new X509Certificate2(certPath);
    }

    private static string GetProjectCertPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var projectDir = new DirectoryInfo(currentDir);

        while (projectDir != null && !projectDir.GetDirectories("src").Any())
            projectDir = projectDir.Parent;

        return projectDir == null
            ? throw new Exception("Project directory not found.")
            : Path.Combine(projectDir.FullName, "src", "certs", "elastic-certificate.crt");
    }
}