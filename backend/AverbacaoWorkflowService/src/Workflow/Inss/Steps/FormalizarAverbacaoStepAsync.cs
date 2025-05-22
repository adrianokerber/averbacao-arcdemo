using AverbacaoWorkflowService.Workflow.shared;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace AverbacaoWorkflowService.Workflow.Inss.Steps;

public class FormalizarAverbacaoStepAsync(ILogger<FormalizarAverbacaoStepAsync> logger, IConfiguration configuration) : StepBodyAsync
{
    public int Codigo { get; set; }
    public FlowBehaviour FlowBehaviour { get; set; }
    
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var averbacaoService = configuration.GetSection("AverbacaoServiceUri").Value!;
        logger.LogInformation("Chama micro-serviço AverbacaoService POST:averbacoes/formalizar");
        
        try
        {
            await $"{averbacaoService}/averbacoes/formalizar".PostJsonAsync(new { codigoProposta = Codigo });
            logger.LogInformation("Averbação formalizada com sucesso: {@0}", Codigo);
            FlowBehaviour = FlowBehaviour.Continue;
        }
        catch (FlurlHttpException ex)
        {
            var err = await ex.GetResponseStringAsync();
            logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {err}");
            
            if (ex.Call.Response?.StatusCode == 400)
            {
                logger.LogError("Invalid request (400) - terminating workflow. Error: {Error}", err);
                FlowBehaviour = FlowBehaviour.Terminate;
                return ExecutionResult.Next();
            }
            
            // For other errors, throw to allow retry
            throw new Exception($"Failed to formalize averbacao: {err}");
        }
        
        return ExecutionResult.Next();
    }
}