namespace Averbacao.startupInfra.Kafka;

public static class MessageTypeRegistry
{
    private static readonly Dictionary<string, Type> MessageTypes = new()
    {
        { "PropostaAverbacaoOrquestradorMessage", typeof(AverbacaoMessage) }
    };

    private static readonly Dictionary<string, Type> ConsumerTypes = new()
    {
        { "RecepcionarItencaoAverbacaoEventConsumer", typeof(RecepcionarItencaoAverbacaoEventConsumer) }
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