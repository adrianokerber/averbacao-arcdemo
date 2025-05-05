namespace AverbacaoService.shared.EfUow;

public interface IEfDbContextFactory<T> where T : Microsoft.EntityFrameworkCore.DbContext
{
    Task<T> CriarAsync();
}