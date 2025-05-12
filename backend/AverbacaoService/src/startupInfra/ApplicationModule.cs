using Autofac;
using AverbacaoService.shared;
using AverbacaoService.shared.DatabaseDetails;
using AverbacaoService.shared.DatabaseDetails.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AverbacaoService.startupInfra;

public class ApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsClosedTypesOf(typeof(IService<>))
            .InstancePerLifetimeScope();

        // EntityFramework configuration
        builder.Register(c =>
        {
            var config = c.Resolve<IConfiguration>();
            var connectionString = config.GetSection("Database:ConnectionString").Value;
            
            var optionsBuilder = new DbContextOptionsBuilder<AverbacaoDbContext>()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseSqlServer(connectionString, options => options.EnableRetryOnFailure())
                .LogTo(Console.WriteLine, LogLevel.Information);

            return optionsBuilder.Options;
        })
        .As<DbContextOptions<AverbacaoDbContext>>()
        .SingleInstance();

        builder.Register(c => 
        {
            var options = c.Resolve<DbContextOptions<AverbacaoDbContext>>();
            return new AverbacaoDbContext(options);
        })
        .As<AverbacaoDbContext>()
        .InstancePerLifetimeScope();

        // TODO: remove commented code and unused classes since we couldn't make them work
        // builder
        //     .RegisterType<AverbacaoDbContextFactory>()
        //     .As<IEfDbContextFactory<AverbacaoDbContext>>()
        //     .InstancePerLifetimeScope();
        //
        // builder
        //     .RegisterType<AverbacaoDbContextAccessor>()
        //     .As<IEfDbContextAccessor<AverbacaoDbContext>>()
        //     .InstancePerLifetimeScope();
        // builder
        //     .RegisterType<EfUnitOfWork>()
        //     .As<IUnitOfWork>()
        //     .InstancePerLifetimeScope();

        builder
            .RegisterType<HttpContextAccessor>()
            .As<IHttpContextAccessor>()
            .InstancePerLifetimeScope();
    }
}