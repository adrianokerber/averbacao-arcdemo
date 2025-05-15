using AverbacaoService.Infrastructure.EfContextConfiguration.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AverbacaoService.Infrastructure.EfContextConfiguration
{
    [Obsolete]
    public sealed class AverbacaoDbContextFactory(IConfiguration configuration) : IEfDbContextFactory<AverbacaoDbContext>
    {
        public async Task<AverbacaoDbContext> CriarAsync()
        {
            var connectionString = configuration.GetSection("Database:ConnectionString").Value;

            var options = new DbContextOptionsBuilder<AverbacaoDbContext>()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseSqlServer(connectionString, options => options.EnableRetryOnFailure())
                .LogTo(Console.WriteLine, LogLevel.Information)
                .Options;
            return new AverbacaoDbContext(options);
        }
    }
}