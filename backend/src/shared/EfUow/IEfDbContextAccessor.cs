namespace Averbacao.modules.Inss.Infraestructure.UoW
{
    public interface IEfDbContextAccessor<T> : IDisposable where T : Microsoft.EntityFrameworkCore.DbContext
    {
        void Register(T context);
        T Get();
        void Clear();
    }
}