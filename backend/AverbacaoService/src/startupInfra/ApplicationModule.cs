using Autofac;
using AverbacaoService.shared;
using AverbacaoService.shared.DatabaseDetails;
using AverbacaoService.shared.DatabaseDetails.Interfaces;

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
        builder
            .RegisterType<AverbacaoDbContextFactory>()
            .As<IEfDbContextFactory<AverbacaoDbContext>>()
            .InstancePerLifetimeScope();
        builder
            .RegisterType<AverbacaoDbContextAccessor>()
            .As<IEfDbContextAccessor<AverbacaoDbContext>>()
            .InstancePerLifetimeScope();
        builder
            .RegisterType<EfUnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<HttpContextAccessor>()
            .As<IHttpContextAccessor>()
            .InstancePerLifetimeScope();
    }
}