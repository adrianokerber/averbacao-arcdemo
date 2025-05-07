using AverbacaoService.shared.DatabaseDetails.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AverbacaoService.shared.DatabaseDetails
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