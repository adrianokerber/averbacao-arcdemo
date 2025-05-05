using AverbacaoService.shared.DbContext;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AverbacaoService.Domain.Averbacoes;

public class AverbacoesRepository(AverbacaoDbContextAccessor dbContextAccessor, ILogger<AverbacoesRepository> logger)
{
    public async Task SalvarAlteracoes(Averbacao averbacao, CancellationToken cancellationToken)
    {
        await dbContextAccessor.Get()
                               .SaveChangesAsync();
    }

    public async Task<Maybe<Averbacao>> ObterPorProposta(int propostaCodigo)
    {
        return await dbContextAccessor.Get()
                                         .Averbacoes
                                         .FirstOrDefaultAsync(a => a.Proposta.Codigo == propostaCodigo);
    }

    public async Task<Guid> Incluir(Averbacao averbacao, CancellationToken cancellationToken)
    {
        dbContextAccessor.Get()
                         .Add(averbacao);
        
        var changesSaved = dbContextAccessor.Get().SaveChanges() > 0;
        if (changesSaved)
            return averbacao.Id;

        return Guid.Empty;
    }
}