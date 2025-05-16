using AverbacaoService.Infrastructure.EfContextConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AverbacaoService.shared;
using AverbacaoService.StartupInfra;
using Microsoft.AspNetCore.Http;

namespace AverbacaoService.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove any existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AverbacaoDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);
            
            // Remove the application module that would contain the real DB registration
            var autofacModuleDescriptor = services.SingleOrDefault(
                d => d.ImplementationType == typeof(ApplicationModule));
            if (autofacModuleDescriptor != null)
                services.Remove(autofacModuleDescriptor);
        });

        // Update with test configuration
        builder.UseEnvironment("Test");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Configure Autofac
        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                // Register modified application module without real database
                containerBuilder.RegisterModule(new TestApplicationModule());

                // Override the DbContextOptions with in-memory version
                containerBuilder.Register(context =>
                {
                    var options = new DbContextOptionsBuilder<AverbacaoDbContext>()
                        .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                        .EnableSensitiveDataLogging()
                        .Options;
                    return options;
                })
                .As<DbContextOptions<AverbacaoDbContext>>()
                .SingleInstance();

                // Override the DbContext itself
                containerBuilder.Register(context =>
                {
                    var options = context.Resolve<DbContextOptions<AverbacaoDbContext>>();
                    var dbContext = new AverbacaoDbContext(options);
                    return dbContext;
                })
                .As<AverbacaoDbContext>()
                .InstancePerLifetimeScope();
            });

        var host = base.CreateHost(builder);

        // Initialize the database
        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AverbacaoDbContext>();
            // Ensure database is created and ready
            dbContext.Database.EnsureCreated();
        }

        return host;
    }
}

// Custom module for testing that doesn't include real database connection
public class TestApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsClosedTypesOf(typeof(IService<>))
            .InstancePerLifetimeScope();

        builder
            .RegisterType<HttpContextAccessor>()
            .As<IHttpContextAccessor>()
            .InstancePerLifetimeScope();
    }
}
