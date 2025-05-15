namespace AverbacaoService.Infrastructure.EfContextConfiguration.Interfaces;

[Obsolete]
public interface IEfDbContextFactory<T> where T : Microsoft.EntityFrameworkCore.DbContext
{
    Task<T> CriarAsync();
}