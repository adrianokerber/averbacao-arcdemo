using AverbacaoService.shared.DatabaseDetails.Interfaces;

namespace AverbacaoService.shared.DatabaseDetails
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
