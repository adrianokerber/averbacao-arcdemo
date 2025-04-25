namespace Averbacao.modules.Inss.Infraestructure.UoW;

public interface IEfDbContextFactory<T> where T : Microsoft.EntityFrameworkCore.DbContext
{
    Task<T> CriarAsync();
}