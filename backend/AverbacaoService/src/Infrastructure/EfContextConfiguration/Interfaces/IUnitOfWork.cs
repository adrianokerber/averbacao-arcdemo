namespace AverbacaoService.Infrastructure.EfContextConfiguration.Interfaces
{
    [Obsolete]
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken cancellationToken);
    }
}