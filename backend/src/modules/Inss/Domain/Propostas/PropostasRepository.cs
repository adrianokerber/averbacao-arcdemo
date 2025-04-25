using Averbacao.modules.Inss.Infraestructure.DbContext;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Averbacao.modules.Inss.Domain.Propostas;

public class PropostasRepository(AverbacaoDbContext dbContext, ILogger<PropostasRepository> logger)
{
    // TODO: implementar método salvar
    public async Task<Result<Proposta>> SalvarProposta(Proposta proposta, CancellationToken cancellationToken)
    {
        return Result.Success(proposta);
    }
}