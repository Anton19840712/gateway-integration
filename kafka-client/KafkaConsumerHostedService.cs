using System.Text;
using Kafka.Public.Loggers;
using Kafka.Public;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace kafka_client;

public class KafkaConsumerHostedService : IHostedService
{
    private readonly ILogger<KafkaConsumerHostedService> _logger;
    private readonly ClusterClient _cluster;

    public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger)
    {
        _logger = logger;
        _cluster = new ClusterClient(new Configuration
        {
            Seeds = "localhost:9092"
        }, new ConsoleLogger());
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cluster.ConsumeFromLatest("demo");
        _cluster.MessageReceived += record =>
        {
            _logger.LogInformation($"Consumed: {Encoding.UTF8.GetString((record.Value as byte[])!)}");
            //Console.WriteLine($"Consumed: {Encoding.UTF8.GetString((record.Value as byte[])!)}");
        };

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cluster.Dispose();
        return Task.CompletedTask;
    }
}