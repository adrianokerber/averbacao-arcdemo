using Flurl.Http;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace AverbacaoWorkflowService.Workflow.Inss;

public class InclusaoInssWorkflowDefinition : IWorkflow<PropostaInssData>
{
    public string Id => "InclusaoInssWorkflowDefinition";
    public int Version => 1;

    public void Build(IWorkflowBuilder<PropostaInssData> builder)
    {    
        builder
            .StartWith<CriarAverbacaoStepAsync>()
                .Input(step => step.IntencaoProposta, data => data)
            .Then<FormalizarAverbacaoStepAsync>()
                .Input(step => step.Codigo, data => data.Codigo)
            .Then<InformarSistemaLegadoStepAsync>();
    }
}

public class CriarAverbacaoStepAsync(ILogger<CriarAverbacaoStepAsync> logger) : StepBodyAsync
{
    public PropostaInssData IntencaoProposta { get; set; } 
    
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        logger.LogInformation("Chama micro-serviço AverbacaoService POST:averbacoes/criar");
        
        try
        {
            // TODO: url deve ser lida de configs/appsettings
            await "http://averbacao-service/averbacoes/criar".PostJsonAsync(IntencaoProposta);
            logger.LogInformation("Averbação recebida com sucesso: {@0}", IntencaoProposta);
        }
        catch (FlurlHttpException ex)
        {
            var err = await ex.GetResponseStringAsync();
            logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {err}");
            
            throw new Exception("Invalid message. Cannot process.");
        }
        
        return ExecutionResult.Next();
    }
}

public class FormalizarAverbacaoStepAsync(ILogger<FormalizarAverbacaoStepAsync> logger) : StepBodyAsync
{
    public int Codigo { get; set; }
    
    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        logger.LogInformation("Chama micro-serviço AverbacaoService POST:averbacoes/formalizar");
        
        try
        {
            // TODO: url deve ser lida de configs/appsettings
            await "http://averbacao-service/averbacoes/formalizar".PostJsonAsync(Codigo);
            logger.LogInformation("Averbação recebida com sucesso: {@0}", Codigo);
        }
        catch (FlurlHttpException ex)
        {
            var err = await ex.GetResponseStringAsync();
            logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {err}");
            
            throw new Exception("Invalid message. Cannot process.");
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

public class PropostaInssData
{
    public int Codigo { get; set; }
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