using AverbacaoWorkflowService.Workflow.Inss.Steps;
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
            .Decide(data => data.StepResultBehaviour)
                .When(WorkflowErrorHandling.Terminate)
                    .Then<HandleInvalidRequestStepAsync>()
                .When(WorkflowErrorHandling.Retry) // NOTE: this is equivalent here to a CONTINUE. We must later change the control flow operator for another type - maybe a record structured like a enum  
                    .Then<FormalizarAverbacaoStepAsync>()
                        .Input(step => step.Codigo, data => data.Proposta.Codigo)
                        .Output(data => data.StepResultBehaviour, step => step.StepResultBehaviour)
                    .Decide(data => data.StepResultBehaviour)
                        .When(WorkflowErrorHandling.Terminate)
                            .Then<HandleInvalidRequestStepAsync>()
                        .When(WorkflowErrorHandling.Retry)
                            .Then<InformarSistemaLegadoStepAsync>();
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