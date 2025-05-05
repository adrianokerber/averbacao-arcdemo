using AverbacaoService.shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace AverbacaoService.Domain.Averbacoes.Features.Criar.Application;

public class CriarAverbacaoConsumer(ILogger<CriarAverbacaoConsumer> logger, CriarCommandHandler handler)
{
    public async Task OnMessageReceivedAsync(PropostaAverbacaoMessage message)
    {
        var cpf = Cpf.Criar(message.Proponente.Cpf);
        if (cpf.IsFailure)
            throw new ConsumerFatalException("Invalid message. Cannot process.");
        
        var proponente = new Proponente(cpf.Value, message.Proponente.Nome, message.Proponente.Sobrenome,
            message.Proponente.DataNascimento);
        var prazo = new Prazo(message.PrazoEmMeses);
        var proposta = new Proposta(message.Codigo, proponente, message.Valor, prazo);
        
        var command = CriarCommand.Criar(proposta);
        if (command.IsFailure)
            throw new ConsumerFatalException("Invalid message. Cannot process.");

        var averbacao = await handler.HandleAsync(command.Value);
        
        logger.LogInformation($"Averbacao criada com sucesso: {averbacao}");
    }
}

public record PropostaAverbacaoMessage(int Codigo, ProponenteAverbacaoMessage Proponente, decimal Valor, int PrazoEmMeses);
public record ProponenteAverbacaoMessage(string Cpf, string Nome, string Sobrenome, DateTime DataNascimento);

public class ConsumerFatalException(string message) : Exception(message);