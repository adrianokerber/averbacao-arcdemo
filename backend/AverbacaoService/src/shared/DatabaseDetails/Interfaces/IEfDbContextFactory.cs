namespace AverbacaoService.shared.DatabaseDetails.Interfaces;

public interface IEfDbContextFactory<T> where T : Microsoft.EntityFrameworkCore.DbContext
{
    Task<T> CriarAsync();
}