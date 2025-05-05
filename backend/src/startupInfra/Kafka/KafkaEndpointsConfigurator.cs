using Confluent.Kafka;
using Silverback.Messaging.Configuration;
using Silverback.Messaging.Serialization;

namespace AverbacaoService.startupInfra.Kafka;

public class KafkaEndpointsConfigurator : IEndpointsConfigurator
{
    private readonly KafkaConfig _kafkaConfig;

    public KafkaEndpointsConfigurator(KafkaConfig configuration)
    {
        _kafkaConfig = configuration;
    }

    //https://silverback-messaging.net/
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder.AddKafkaEndpoints(endpoints =>
        {
            endpoints.Configure(config => { config.BootstrapServers = _kafkaConfig.BootstrapServers; });

            foreach (var inbound in _kafkaConfig.Consumer.Inbounds)
            {
                var messageType = MessageTypeRegistry.GetMessageType(inbound.MessageType);
                var consumerType = MessageTypeRegistry.GetConsumerType(inbound.ConsumerType);
                if (messageType == null)
                {
                    throw new InvalidOperationException($"Message type '{inbound.MessageType}' not found.");
                }

                if (!Enum.TryParse<AutoOffsetReset>(inbound.AutoOffsetReset, true, out var autoOffsetReset))
                {
                    throw new ArgumentException($"Invalid AutoOffsetReset value: {inbound.AutoOffsetReset}");
                }

                var serializerType = typeof(NewtonsoftJsonMessageSerializer<>).MakeGenericType(messageType);
                var serializer = Activator.CreateInstance(serializerType) as IMessageSerializer
                                 ?? throw new InvalidOperationException(
                                     $"Failed to create serializer for type {messageType}");

                endpoints.AddInbound(messageType, endpoint =>
                {
                    endpoint.SkipNullMessages();
                    endpoint.ConsumeFrom(inbound.Topics)
                        .Configure(config =>
                        {
                            config.GroupId = _kafkaConfig.GroupId;
                            config.AutoOffsetReset = autoOffsetReset;
                        })
                        .OnError(policy =>
                        {
                            policy.Retry(
                                _kafkaConfig.Retry.Attempts,
                                TimeSpan.FromSeconds(_kafkaConfig.Retry.IntervalInSeconds)
                            );
                            policy.MoveToKafkaTopic(moveEndpoint => { moveEndpoint.ProduceTo(inbound.TopicError); });
                            policy.Skip();
                        })
                        .DeserializeUsing(serializer);
                });
            }
        });
    }
}