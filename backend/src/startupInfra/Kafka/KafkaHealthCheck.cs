using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Averbacao.startupInfra.Kafka;

public class KafkaHealthCheck : IHealthCheck
{
    private readonly KafkaConfig _config;

    public KafkaHealthCheck(KafkaConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Configurações básicas para conexão ao Kafka
            var config = new AdminClientConfig
            {
                BootstrapServers = _config.BootstrapServers
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            // Obtém os metadados dos brokers com timeout de 5 segundos
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));

            return Task.FromResult(metadata.Brokers.Count > 0 
                ? HealthCheckResult.Healthy("Kafka is reachable.") 
                : HealthCheckResult.Unhealthy("No Kafka brokers available."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Failed to connect to Kafka: {ex.Message}", ex));
        }
    }
}