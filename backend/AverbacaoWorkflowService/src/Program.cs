using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using AverbacaoWorkflowService.StartupInfra.Extensions;
using AverbacaoWorkflowService.StartupInfra.Kafka;
using AverbacaoWorkflowService.Workflow.Inss;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WorkflowCore.Interface;

var serviceVersion = Environment.GetEnvironmentVariable("DD_VERSION") ??
                     Assembly.GetExecutingAssembly().GetName().Version?.ToString();
var appName = Assembly.GetExecutingAssembly().GetName().Name;
try
{
    Log.ForContext("ApplicationName", appName).Information("Starting application");

    var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
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
            .AddKafka(configuration)
            .AddWorkflow(wo =>
            {
                wo.UseSqlServer(configuration.GetSection("Database:ConnectionString").Value, true, true);
                wo.UseMaxConcurrentWorkflows(10);
            })
            .AddScoped<CriarAverbacaoStepAsync>()
            .AddScoped<FormalizarAverbacaoStepAsync>()
            .AddScoped<InformarSistemaLegadoStepAsync>();
    });
    
    var app = builder.Build();
    builder.AddSerilog(app.Services.GetRequiredService<IConfiguration>());

    app.Services.GetRequiredService<IWorkflowHost>().RegisterWorkflow<InclusaoInssWorkflowDefinition, PropostaInssData>();
    app.Services.GetRequiredService<IWorkflowHost>().Start();

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("Error when trying to start application {0}", ex);
    var errorContext = new
    {
        ApplicationName = appName,
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

