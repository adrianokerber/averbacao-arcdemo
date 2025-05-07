using AverbacaoService.shared;
using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.Averbacoes.Features.Criar;

public class CriarCommandHandler(AverbacoesRepository averbacoesRepository) : IService<CriarCommandHandler>
{
    public async Task<Result<Averbacao>> HandleAsync(CriarCommand command, CancellationToken ct = default)
    {
        var averbacao = await averbacoesRepository.ObterPorProposta(command.Proposta.Codigo);
        if (averbacao.HasValue)
            return Result.Failure<Averbacao>("Averbação já existe");
        
        var averbacaoNova = Averbacao.Criar(command.Proposta);
        if (averbacaoNova.IsFailure)
            return Result.Failure<Averbacao>("Averbação inválida");

        await averbacoesRepository.Incluir(averbacaoNova.Value, ct);
        return averbacaoNova.Value;
    }
}