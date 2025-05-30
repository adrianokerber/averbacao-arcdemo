using AverbacaoWorkflowService.Workflow.Inss;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;

namespace AverbacaoWorkflowService.Entrypoints.Features.IncluirAverbacao.Inss;

public class IncluirAverbacaoInssConsumer(IWorkflowHost workflowHost, ILogger<IncluirAverbacaoInssConsumer> logger)
{
    public async Task OnMessageReceivedAsync(PropostaAverbacaoInssMessage averbacaoInssMessage)
    {
        var inclusaoInss = new PropostaInssData
        {
            Codigo = averbacaoInssMessage.Codigo,
            Convenio = "INSS",
            Proponente = new ProponenteInssData {
                Cpf = averbacaoInssMessage.Proponente.Cpf,
                Nome = averbacaoInssMessage.Proponente.Nome,
                Sobrenome = averbacaoInssMessage.Proponente.Sobrenome,
                DataNascimento = averbacaoInssMessage.Proponente.DataNascimento
            },
            PrazoEmMeses = averbacaoInssMessage.PrazoEmMeses,
            Valor = averbacaoInssMessage.Valor
        };
        await workflowHost.StartWorkflow("InclusaoInssWorkflowDefinition", new InssWorkflowData{ Proposta = inclusaoInss });
    }
}

public record PropostaAverbacaoInssMessage(int Codigo, ProponenteAverbacaoInssMessage Proponente, decimal Valor, int PrazoEmMeses);
public record ProponenteAverbacaoInssMessage(string Cpf, string Nome, string Sobrenome, DateTime DataNascimento);