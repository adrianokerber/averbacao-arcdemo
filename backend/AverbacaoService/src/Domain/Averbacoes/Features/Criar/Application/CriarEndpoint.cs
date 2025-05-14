using AverbacaoService.shared;
using AverbacaoService.shared.ValueObjects;
using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace AverbacaoService.Domain.Averbacoes.Features.Criar.Application;

public class CriarEndpoint(ILogger<CriarEndpoint> logger, CriarCommandHandler handler, HttpResponseFactory httpResponseFactory) : Endpoint<CriarAverbacaoRequest, object>
{
    public override void Configure()
    {
        Post("/averbacoes/criar");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CriarAverbacaoRequest req, CancellationToken ct)
    {
        var cpf = Cpf.Criar(req.Proponente.Cpf);
        if (cpf.IsFailure)
        {
            await SendResultAsync(httpResponseFactory.CreateError400("Invalid message. Cannot process.", cpf.Error));
            return;
        }
        
        var proponente = new Proponente(cpf.Value, req.Proponente.Nome, req.Proponente.Sobrenome,
            req.Proponente.DataNascimento);
        var prazo = new Prazo(req.PrazoEmMeses);
        var proposta = new Proposta(req.Codigo, proponente, req.Valor, prazo);
        
        var command = CriarCommand.Criar(proposta);
        if (command.IsFailure)
        {
            await SendResultAsync(httpResponseFactory.CreateError400("Invalid message. Cannot process.", command.Error));
            return;
        }

        var averbacao = await handler.HandleAsync(command.Value);
        if (averbacao.IsFailure)
        {
            await SendResultAsync(httpResponseFactory.CreateError400("Invalid message. Cannot process.", averbacao.Error));
            return;
        }
        
        logger.LogInformation("Averbacao criada com sucesso: {@0}", averbacao.Value);
        await SendResultAsync(httpResponseFactory.CreateSuccess200(averbacao.Value));
    }
}

public record CriarAverbacaoRequest(int Codigo, ProponenteDto Proponente, decimal Valor, int PrazoEmMeses);
public record ProponenteDto(string Cpf, string Nome, string Sobrenome, DateTime DataNascimento);