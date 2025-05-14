using AverbacaoService.Domain.Averbacoes.Infraestructure;
using AverbacaoService.shared;
using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.Averbacoes.Features.Formalizar;

public class FormalizarCommandHandler(AverbacoesRepository averbacoesRepository, FormalizarAverbacaoService formalizarAverbacaoService) : IService<FormalizarCommandHandler>
{
    public async Task<Result<Averbacao>> HandleAsync(FormalizarCommand command, CancellationToken ct = default)
    {
        var averbacao = await averbacoesRepository.ObterPorPropostaAsync(command.CodigoProposta);
        if (averbacao.HasNoValue)
            return Result.Failure<Averbacao>("Averbação não encontrada para proposta");
        if (averbacao.Value.EstaFormalizada())
            return Result.Failure<Averbacao>("Averbação já formalizada");
        
        var integrationResult = await formalizarAverbacaoService.ExecuteAsync(averbacao.Value.Proposta);
        if (integrationResult.IsFailure)
            return Result.Failure<Averbacao>(integrationResult.Error);
        
        var result = averbacao.Value.Formalizar(integrationResult.Value);
        if (result.IsFailure)
            return Result.Failure<Averbacao>(result.Error);

        await averbacoesRepository.AtualizarAsync(averbacao.Value, ct);
        return averbacao.Value;
    }
}