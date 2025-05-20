using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace AverbacaoWorkflowService.Workflow.Inss.Steps;

public class InformarSistemaLegadoStepAsync(ILogger<InformarSistemaLegadoStepAsync> logger) : StepBodyAsync
{
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        logger.LogInformation("Integra com micro-serviço de integração com legado 'AverbacaoIntegradorLegadoService'");
        return ExecutionResult.Next();
    }
}