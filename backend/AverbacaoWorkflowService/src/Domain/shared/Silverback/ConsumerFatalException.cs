namespace AverbacaoWorkflowService.Domain.shared.Silverback;

public class ConsumerFatalException(string message) : Exception(message);