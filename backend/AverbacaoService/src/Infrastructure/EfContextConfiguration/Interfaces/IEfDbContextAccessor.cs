namespace AverbacaoService.Infrastructure.EfContextConfiguration.Interfaces
{
    [Obsolete]
    public interface IEfDbContextAccessor<T> : IDisposable where T : Microsoft.EntityFrameworkCore.DbContext
    {
        void Register(T context);
        T Get();
        void Clear();
    }
}