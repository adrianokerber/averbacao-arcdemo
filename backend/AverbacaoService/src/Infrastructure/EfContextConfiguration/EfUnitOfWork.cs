using AverbacaoService.Infrastructure.EfContextConfiguration.Interfaces;

namespace AverbacaoService.Infrastructure.EfContextConfiguration
{
    [Obsolete]
    public class EfUnitOfWork(IEfDbContextAccessor<AverbacaoDbContext> efDbContextAccessor) : IUnitOfWork
    {
        public async Task Commit(CancellationToken cancellationToken)
        {
            await efDbContextAccessor.Get().SaveChangesAsync(cancellationToken);
        }
    }
}
