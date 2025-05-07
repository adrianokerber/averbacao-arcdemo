using System.Reflection;
using AverbacaoService.shared;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Exceptions;
using Serilog.Filters;

namespace AverbacaoService.startupInfra;

internal static class ServicesExtensions
{
    public static IServiceCollection AddLogs(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Hosting.Diagnostics"))
            .Filter.ByExcluding(Matching.FromSource("Microsoft.Hosting.Lifetime"))
            .Filter.ByExcluding(
                Matching.FromSource("Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager")
            )
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        services.AddSingleton(Log.Logger);
        return services;
    }
    
    public static IServiceCollection AddOpenApiSpecs(this IServiceCollection services)
    {
        services.AddOpenApiDocument();
        return services;
    }
    
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(
            o =>
                o.AddPolicy(
                    "default",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                )
        );

        return services;
    }

    public static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();
        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "ready" });
        
        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
        var assembly = Assembly.Load(typeof(Program).Assembly.ToString()); // Note: This is useful when loading from another assembly, here we are using like these just as an example
        services
            .AddControllers()
            .AddApplicationPart(assembly);
        return services;
    }

    public static IServiceCollection AddHttpGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<HttpGlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}