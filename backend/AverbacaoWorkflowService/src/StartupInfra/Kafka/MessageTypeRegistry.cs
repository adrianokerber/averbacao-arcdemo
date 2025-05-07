using AverbacaoWorkflowService.Domain.Features.IncluirAverbacao.Inss;

namespace AverbacaoWorkflowService.StartupInfra.Kafka;

public static class MessageTypeRegistry
{
    private static readonly Dictionary<string, Type> MessageTypes = new()
    {
        { nameof(PropostaAverbacaoInssMessage), typeof(PropostaAverbacaoInssMessage) }
    };

    private static readonly Dictionary<string, Type> ConsumerTypes = new()
    {
        { nameof(IncluirAverbacaoInssConsumer), typeof(IncluirAverbacaoInssConsumer) }
    };

    public static Type GetMessageType(string messageTypeName)
    {
        if (!MessageTypes.TryGetValue(messageTypeName, out var type))
        {
            throw new InvalidOperationException($"Message type '{messageTypeName}' not found in registry.");
        }

        return type;
    }

    public static Type GetConsumerType(string consumerTypeName)
    {
        if (!ConsumerTypes.TryGetValue(consumerTypeName, out var type))
        {
            throw new InvalidOperationException($"Consumer type '{consumerTypeName}' not found in registry.");
        }

        return type;
    }
}