namespace AverbacaoWorkflowService.Workflow.shared;

public record FlowBehaviour(string Value)
{
    public static FlowBehaviour Continue => new FlowBehaviour("Continue");
    public static FlowBehaviour Terminate => new FlowBehaviour("Terminate");
}