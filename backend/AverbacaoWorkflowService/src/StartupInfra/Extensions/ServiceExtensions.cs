using System.Reflection;
using AverbacaoWorkflowService.Domain.Features.IncluirAverbacao.Inss;
using AverbacaoWorkflowService.StartupInfra.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;

namespace AverbacaoWorkflowService.StartupInfra.Extensions;

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
            .AddScopedSubscriber<IncluirAverbacaoInssConsumer>();

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
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        });
    }
}