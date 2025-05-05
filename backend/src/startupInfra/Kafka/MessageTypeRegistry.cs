using AverbacaoService.Domain.Averbacoes.Features.Criar.Application;

namespace AverbacaoService.startupInfra.Kafka;

public static class MessageTypeRegistry
{
    private static readonly Dictionary<string, Type> MessageTypes = new()
    {
        { nameof(PropostaAverbacaoMessage), typeof(PropostaAverbacaoMessage) }
    };

    private static readonly Dictionary<string, Type> ConsumerTypes = new()
    {
        { nameof(CriarAverbacaoConsumer), typeof(CriarAverbacaoConsumer) }
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