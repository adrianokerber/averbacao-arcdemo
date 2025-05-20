using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace AverbacaoWorkflowService.Workflow.Inss.Steps;

public class HandleInvalidRequestStepAsync(ILogger<HandleInvalidRequestStepAsync> logger) : StepBodyAsync
{
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        logger.LogInformation("Workflow terminated due to invalid request - no further steps will be executed");
        return ExecutionResult.Next();
    }
}