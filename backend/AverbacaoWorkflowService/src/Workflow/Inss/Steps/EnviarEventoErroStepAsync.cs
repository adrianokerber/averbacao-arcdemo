using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace AverbacaoWorkflowService.Workflow.Inss.Steps;

public class EnviarEventoErroStepAsync(ILogger<EnviarEventoErroStepAsync> logger) : StepBodyAsync
{
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        logger.LogInformation("Erro na averbação enviado a tópico Kafka de saída!"); // Aqui deveríamos ter um produtor Kafka para enviar isso ao tópico de saída destinado ao demandante.
        return ExecutionResult.Next();
    }
}