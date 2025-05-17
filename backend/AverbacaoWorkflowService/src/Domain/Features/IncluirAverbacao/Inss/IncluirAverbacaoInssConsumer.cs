using AverbacaoWorkflowService.Domain.shared.Silverback;
using AverbacaoWorkflowService.Workflow.Inss;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;

namespace AverbacaoWorkflowService.Domain.Features.IncluirAverbacao.Inss;

public class IncluirAverbacaoInssConsumer(IWorkflowHost workflowHost, ILogger<IncluirAverbacaoInssConsumer> logger)
{
    public async Task OnMessageReceivedAsync(PropostaAverbacaoInssMessage averbacaoInssMessage)
    {
        var inclusaoInss = new PropostaInssData
        {
            Codigo = averbacaoInssMessage.Codigo,
            Proponente = new ProponenteInssData {
                Cpf = averbacaoInssMessage.Proponente.Cpf,
                Nome = averbacaoInssMessage.Proponente.Nome,
                Sobrenome = averbacaoInssMessage.Proponente.Sobrenome,
                DataNascimento = averbacaoInssMessage.Proponente.DataNascimento
            },
            PrazoEmMeses = averbacaoInssMessage.PrazoEmMeses,
            Valor = averbacaoInssMessage.Valor
        };
        await workflowHost.StartWorkflow("InclusaoInssWorkflowDefinition", inclusaoInss);
    }
}

public record PropostaAverbacaoInssMessage(int Codigo, ProponenteAverbacaoInssMessage Proponente, decimal Valor, int PrazoEmMeses);
public record ProponenteAverbacaoInssMessage(string Cpf, string Nome, string Sobrenome, DateTime DataNascimento);