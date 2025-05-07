namespace AverbacaoWorkflowService.StartupInfra.Kafka;

public record KafkaConfig
{
    public string? BootstrapServers { get; init; }
    public string? GroupId { get; init; }
    public KafkaConsumerConfig Consumer { get; init; }
    public KafkaRetryConfig Retry { get; init; }

    public KafkaConfig() {}

    public KafkaConfig(string? bootstrapServers, string? groupId, KafkaConsumerConfig consumer, KafkaRetryConfig retry)
    {
        BootstrapServers = bootstrapServers;
        GroupId = groupId;
        Consumer = consumer;
        Retry = retry;
    }
}

public record KafkaConsumerConfig
{
    public List<KafkaInboundConfig> Inbounds { get; init; }

    public KafkaConsumerConfig() {}

    public KafkaConsumerConfig(List<KafkaInboundConfig> inbounds)
    {
        Inbounds = inbounds;
    }
}

public record KafkaInboundConfig
{
    public string MessageType { get; init; }
    public string ConsumerType { get; init; }
    public string Topics { get; init; }
    public string AutoOffsetReset { get; init; }
    public string TopicError { get; init; }

    public KafkaInboundConfig() {}

    public KafkaInboundConfig(string messageType, string consumerType, string topics, string autoOffsetReset, string topicError)
    {
        MessageType = messageType;
        ConsumerType = consumerType;
        Topics = topics;
        AutoOffsetReset = autoOffsetReset;
        TopicError = topicError;
    }
}

public record KafkaRetryConfig
{
    public int Attempts { get; init; }
    public int IntervalInSeconds { get; init; }

    public KafkaRetryConfig() {}

    public KafkaRetryConfig(int attempts, int intervalInSeconds)
    {
        Attempts = attempts;
        IntervalInSeconds = intervalInSeconds;
    }
}