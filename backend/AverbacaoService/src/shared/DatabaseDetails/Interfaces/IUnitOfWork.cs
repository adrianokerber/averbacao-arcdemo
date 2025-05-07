namespace AverbacaoService.shared.DatabaseDetails.Interfaces
{
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken cancellationToken);
    }
}