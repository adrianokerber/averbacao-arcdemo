using AverbacaoWorkflowService.Domain.shared.Silverback;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace AverbacaoWorkflowService.Domain.Features.IncluirAverbacao.Inss;

public class IncluirAverbacaoInssConsumer(ILogger<IncluirAverbacaoInssConsumer> logger)
{
    public async Task OnMessageReceivedAsync(PropostaAverbacaoInssMessage averbacaoInssMessage)
    {
        // TODO: adicionar Workflow.Core e trigger abaixo
        // TODO: mover o conteúdo abaixo para um step do Workflow
        try
        {
            // TODO: url deve ser lida de configs/appsettings
            await "http://averbacao-service/averbacoes/criar".PostJsonAsync(averbacaoInssMessage);
            logger.LogInformation($"Averbação recebida com sucesso: {averbacaoInssMessage}");
        }
        catch (FlurlHttpException ex)
        {
            var err = await ex.GetResponseStringAsync();
            logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {err}");
            
            throw new ConsumerFatalException("Invalid message. Cannot process.");
        }
    }
}

public record PropostaAverbacaoInssMessage(int Codigo, ProponenteAverbacaoInssMessage Proponente, decimal Valor, int PrazoEmMeses);
public record ProponenteAverbacaoInssMessage(string Cpf, string Nome, string Sobrenome, DateTime DataNascimento);