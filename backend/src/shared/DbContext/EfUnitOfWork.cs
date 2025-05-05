using AverbacaoService.shared.EfUow;

namespace AverbacaoService.shared.DbContext
{
    public class EfUnitOfWork(IEfDbContextAccessor<AverbacaoDbContext> efDbContextAccessor) : IUnitOfWork
    {
        public async Task Commit(CancellationToken cancellationToken)
        {
            await efDbContextAccessor.Get().SaveChangesAsync(cancellationToken);
        }
    }
}
