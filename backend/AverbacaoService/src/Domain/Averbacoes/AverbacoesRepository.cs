using AverbacaoService.shared;
using AverbacaoService.shared.DatabaseDetails;
using AverbacaoService.shared.DatabaseDetails.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace AverbacaoService.Domain.Averbacoes;

public class AverbacoesRepository(AverbacaoDbContext dbContext, ILogger<AverbacoesRepository> logger) : IService<AverbacoesRepository>
{
    public async Task SalvarAlteracoes(Averbacao averbacao, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<Maybe<Averbacao>> ObterPorProposta(int propostaCodigo)
    {
        return await dbContext
            .Averbacoes
            .FirstOrDefaultAsync(a => a.Proposta.Codigo == propostaCodigo);
    }

    public async Task<Guid> Incluir(Averbacao averbacao, CancellationToken cancellationToken)
    {
        dbContext.Add(averbacao);
        
        var changesSaved = dbContext.SaveChanges() > 0;
        if (changesSaved)
            return averbacao.Id;

        return Guid.Empty;
    }
}