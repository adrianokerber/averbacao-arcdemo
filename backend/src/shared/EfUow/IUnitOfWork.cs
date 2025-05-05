namespace AverbacaoService.shared.EfUow
{
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken cancellationToken);
    }
}