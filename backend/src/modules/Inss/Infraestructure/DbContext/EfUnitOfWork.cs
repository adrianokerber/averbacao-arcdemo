using Averbacao.modules.Inss.Infraestructure.UoW;

namespace Averbacao.modules.Inss.Infraestructure.DbContext
{
    public class EfUnitOfWork(IEfDbContextAccessor<AverbacaoDbContext> efDbContextAccessor) : IUnitOfWork
    {
        public async Task Commit(CancellationToken cancellationToken)
        {
            await efDbContextAccessor.Get().SaveChangesAsync(cancellationToken);
        }
    }
}
