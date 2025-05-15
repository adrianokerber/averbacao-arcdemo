using AverbacaoService.Domain.shared.ValueObjects;
using AverbacaoService.shared;
using CSharpFunctionalExtensions;
using Flurl.Http;

namespace AverbacaoService.Domain.Averbacoes.Infraestructure;

public class FormalizarAverbacaoService(ILogger<FormalizarAverbacaoService> logger) : IService<FormalizarAverbacaoService>
{
    public Task<Result<Formalizacao>> ExecuteAsync(Proposta proposta)
    {
        // In a real case cenario I would do the integration with the partner here. Like Dataprev API for INSS for example...
        
        logger.LogInformation("Proposta averbada no parceiro -> {@0}", proposta);
        return Task.FromResult<Result<Formalizacao>>(new Formalizacao(33, DateTime.UtcNow, "Averbado na DataPrev com sucesso, seu contrato está ativo!"));
    }
}