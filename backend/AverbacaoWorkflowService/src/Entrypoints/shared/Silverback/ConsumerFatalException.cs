namespace AverbacaoWorkflowService.Entrypoints.shared.Silverback;

public class ConsumerFatalException(string message) : Exception(message);