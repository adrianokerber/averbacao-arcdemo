using AverbacaoService.shared;
using AverbacaoService.shared.DatabaseDetails;
using AverbacaoService.shared.DatabaseDetails.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AverbacaoService.Domain.Averbacoes;

public class AverbacoesRepository(AverbacaoDbContext dbContext, ILogger<AverbacoesRepository> logger) : IService<IAverbacoesRepository>
{
    public async Task<Maybe<Averbacao>> ObterPorProposta(int propostaCodigo)
    {
        return await dbContext
            .Averbacoes
            .FirstOrDefaultAsync(a => a.Proposta.Codigo == propostaCodigo);
    }

    public async Task<Guid> IncluirAsync(Averbacao averbacao, CancellationToken ct = default)
    {
        dbContext.Add(averbacao);
        
        var changesSaved = await dbContext.SaveChangesAsync(ct) > 0;
        if (changesSaved)
            return averbacao.Id;

        return Guid.Empty;
    }

    public async Task<int> SalvarAlteracoesAsync(CancellationToken ct = default)
    {
        return await dbContext.SaveChangesAsync(ct);
    }

    public async Task<Maybe<Averbacao>> ObterPorIdAsync(Guid id)
    {
        return await dbContext.Averbacoes.FindAsync(id);
    }

    public async Task<IEnumerable<Averbacao>> ObterTodasAsync()
    {
        return await dbContext.Averbacoes.ToListAsync();
    }
}