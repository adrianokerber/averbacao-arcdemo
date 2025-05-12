namespace AverbacaoService.shared.DatabaseDetails.Interfaces;

[Obsolete]
public interface IEfDbContextFactory<T> where T : Microsoft.EntityFrameworkCore.DbContext
{
    Task<T> CriarAsync();
}