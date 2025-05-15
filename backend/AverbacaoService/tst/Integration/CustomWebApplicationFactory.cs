using AverbacaoService.Infrastructure.EfContextConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AverbacaoService.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => 
                d.ServiceType == typeof(DbContextOptions<AverbacaoDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AverbacaoDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "TestDb"));
        });
    }
}
