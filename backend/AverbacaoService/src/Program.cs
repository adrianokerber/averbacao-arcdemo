using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AverbacaoService.Infrastructure;
using AverbacaoService.StartupInfra;
using CSharpFunctionalExtensions;
using FastEndpoints;
using Serilog;

namespace AverbacaoService;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        var appName = assemblyName.Name;
        var appVersion = Environment.GetEnvironmentVariable("DD_VERSION") ??
                         Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        try
        {
            Log.ForContext("ServiceDetails", new { appName, appVersion })
               .Information("Starting service");
            Result.Configuration.ErrorMessagesSeparator = "§ ";
            
            builder.Services
                .AddLogs(builder.Configuration)
                .AddEndpointsApiExplorer()
                .AddFastEndpoints(o
                    => o.SourceGeneratorDiscoveredTypes.AddRange(typeof(Program).Assembly.GetTypes()))
                .AddOpenApiSpecs()
                .AddHttpGlobalExceptionHandler();
            
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new ApplicationModule());
            });
            builder.Host.UseSerilog();
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            var app = builder.Build();

            app.UseFastEndpoints();
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                // Add OpenAPI 3.0 document serving middleware
                // Available at: http://localhost:<port>/swagger/v1/swagger.json
                app.UseOpenApi();

                // Add web UIs to interact with the document
                // Available at: http://localhost:<port>/swagger
                app.UseSwaggerUi(); // UseSwaggerUI Protected by if (env.IsDevelopment())
                
                // Initialize the database with seed data
                await SeedData.Initialize(app.Services);
            }

            app.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.ForContext("ApplicationName", appName)
                .Fatal(ex, "Program terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
