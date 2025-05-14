using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.Averbacoes;

public interface IAverbacoesRepository
{
    Task<Guid> IncluirAsync(Averbacao averbacao, CancellationToken ct = default);
    Task<Result> AtualizarAsync(Averbacao averbacao, CancellationToken ct = default);
    Task<int> SalvarAlteracoesAsync(CancellationToken ct = default);
    Task<Maybe<Averbacao>> ObterPorProposta(int propostaCodigo);
    Task<Maybe<Averbacao>> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Averbacao>> ObterTodasAsync();
}
