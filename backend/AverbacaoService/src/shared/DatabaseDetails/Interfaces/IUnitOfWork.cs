namespace AverbacaoService.shared.DatabaseDetails.Interfaces
{
    [Obsolete]
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken cancellationToken);
    }
}