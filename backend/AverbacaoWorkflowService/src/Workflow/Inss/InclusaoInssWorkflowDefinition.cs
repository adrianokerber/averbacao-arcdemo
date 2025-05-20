using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace AverbacaoWorkflowService.Workflow.Inss;

public class InclusaoInssWorkflowDefinition : IWorkflow<InssWorkflowData>
{
    public string Id => "InclusaoInssWorkflowDefinition";
    public int Version => 1;

    public void Build(IWorkflowBuilder<InssWorkflowData> builder)
    {    
        builder
            .StartWith<CriarAverbacaoStepAsync>()
                .Input(step => step.IntencaoProposta, data => data.Proposta)
                .Output(data => data.StepResultBehaviour, step => step.StepResultBehaviour)
            .Then<FormalizarAverbacaoStepAsync>().CancelCondition(data => data.StepResultBehaviour == WorkflowErrorHandling.Terminate)
                .Input(step => step.Codigo, data => data.Proposta.Codigo)
                .Output(data => data.StepResultBehaviour, step => step.StepResultBehaviour)
            .Then<InformarSistemaLegadoStepAsync>().CancelCondition(data => data.StepResultBehaviour == WorkflowErrorHandling.Terminate);
    }
}

public class CriarAverbacaoStepAsync(ILogger<CriarAverbacaoStepAsync> logger, IConfiguration configuration) : StepBodyAsync
{
    public PropostaInssData IntencaoProposta { get; set; }
    public WorkflowErrorHandling StepResultBehaviour { get; set; }
    
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var averbacaoService = configuration.GetSection("AverbacaoServiceUri").Value!;
        logger.LogInformation("Chama micro-serviço AverbacaoService POST:averbacoes/criar");
        
        try
        {
            await $"{averbacaoService}/averbacoes/criar".PostJsonAsync(IntencaoProposta);
            logger.LogInformation("Averbação recebida com sucesso: {@0}", IntencaoProposta);
        }
        catch (FlurlHttpException ex)
        {
            var err = await ex.GetResponseStringAsync();
            logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {err}");
            
            if (ex.Call.Response?.StatusCode == 400)
            {
                logger.LogError("Invalid request (400) - terminating workflow. Error: {Error}", err);
                StepResultBehaviour = WorkflowErrorHandling.Terminate;
                return ExecutionResult.Next();
            }
            
            // For other errors, throw to allow retry
            throw new Exception($"Failed to create averbacao: {err}");
        }
        
        return ExecutionResult.Next();
    }
}

public class FormalizarAverbacaoStepAsync(ILogger<FormalizarAverbacaoStepAsync> logger, IConfiguration configuration) : StepBodyAsync
{
    public int Codigo { get; set; }
    public WorkflowErrorHandling StepResultBehaviour { get; set; }
    
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var averbacaoService = configuration.GetSection("AverbacaoServiceUri").Value!;
        logger.LogInformation("Chama micro-serviço AverbacaoService POST:averbacoes/formalizar");
        
        try
        {
            await $"{averbacaoService}/averbacoes/formalizar".PostJsonAsync(new { codigoProposta=Codigo });
            logger.LogInformation("Averbação formalizada com sucesso: {@0}", Codigo);
        }
        catch (FlurlHttpException ex)
        {
            var err = await ex.GetResponseStringAsync();
            logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {err}");
            
            if (ex.Call.Response?.StatusCode == 400)
            {
                logger.LogError("Invalid request (400) - terminating workflow. Error: {Error}", err);
                StepResultBehaviour = WorkflowErrorHandling.Terminate;
                return ExecutionResult.Next();
            }
            
            // For other errors, throw to allow retry
            throw new Exception($"Failed to formalize averbacao: {err}");
        }
        
        return ExecutionResult.Next();
    }
}

public class InformarSistemaLegadoStepAsync(ILogger<InformarSistemaLegadoStepAsync> logger) : StepBodyAsync
{
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        logger.LogInformation("Integra com micro-serviço de integração com legado 'AverbacaoIntegradorLegadoService'");
        return ExecutionResult.Next();
    }
}

public class InssWorkflowData
{
    public PropostaInssData Proposta { get; set; }
    public WorkflowErrorHandling StepResultBehaviour { get; set; }
}

public class PropostaInssData
{
    public int Codigo { get; set; }
    public string Convenio { get; set; }
    public ProponenteInssData Proponente { get; set; }
    public decimal Valor { get; set; }
    public int PrazoEmMeses { get; set; }
}

public class ProponenteInssData
{
    public string Cpf { get; set; }
    public string Nome { get; set; }
    public string Sobrenome { get; set; }
    public DateTime DataNascimento { get; set; }
}