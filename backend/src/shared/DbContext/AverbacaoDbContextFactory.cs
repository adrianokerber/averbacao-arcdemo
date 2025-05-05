using AverbacaoService.shared.EfUow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AverbacaoService.shared.DbContext
{
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