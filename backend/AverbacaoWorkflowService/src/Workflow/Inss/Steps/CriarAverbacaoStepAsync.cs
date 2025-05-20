using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace AverbacaoWorkflowService.Workflow.Inss.Steps;

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
            StepResultBehaviour = WorkflowErrorHandling.Retry; // Just to keep the flow since where is no empty value
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