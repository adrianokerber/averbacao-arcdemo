namespace Averbacao.modules.Inss.Infraestructure.UoW
{
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken cancellationToken);
    }
}