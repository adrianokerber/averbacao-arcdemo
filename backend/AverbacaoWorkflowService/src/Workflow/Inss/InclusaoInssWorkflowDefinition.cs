using AverbacaoWorkflowService.Workflow.Inss.Steps;
using AverbacaoWorkflowService.Workflow.shared;
using WorkflowCore.Interface;

namespace AverbacaoWorkflowService.Workflow.Inss;

public class InclusaoInssWorkflowDefinition : IWorkflow<InssWorkflowData>
{
    public string Id => "InclusaoInssWorkflowDefinition";
    public int Version => 1;

    public void Build(IWorkflowBuilder<InssWorkflowData> builder)
    {
        // Inicia o fluxo do workflow
        builder
            .StartWith<CriarAverbacaoStepAsync>()
                .Input(step => step.IntencaoProposta, data => data.Proposta)
                .Output(data => data.FlowBehaviour, step => step.FlowBehaviour)
            .If(data => data.FlowBehaviour == FlowBehaviour.Terminate)
                .Do(then => then.StartWith<EnviarEventoErroStepAsync>().EndWorkflow())
            .Then<FormalizarAverbacaoStepAsync>()
                .Input(step => step.Codigo, data => data.Proposta.Codigo)
                .Output(data => data.FlowBehaviour, step => step.FlowBehaviour)
            .If(data => data.FlowBehaviour == FlowBehaviour.Terminate)
                .Do(then => then.StartWith<EnviarEventoErroStepAsync>().EndWorkflow())
            .Then<InformarSistemaLegadoStepAsync>();
    }
}

public class InssWorkflowData
{
    public PropostaInssData Proposta { get; set; }
    public FlowBehaviour FlowBehaviour { get; set; }
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