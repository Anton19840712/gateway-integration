using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace kafka_server;

public class KafkaProducerHostedService : IHostedService
{
    private readonly ILogger<KafkaProducerHostedService> _logger;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerHostedService(ILogger<KafkaProducerHostedService> logger)
    {
        _logger = logger;
        var config = new ProducerConfig()
        {
            BootstrapServers = "localhost:9092"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    /// <summary>
    /// Here we produce sample messages to push them to kafka and then read from it by consumer.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < 30; ++i)
        {
            var value = $"Message {i}";
            _logger.LogInformation($"Produced: {value}");
           //Console.WriteLine($"Produced: {value}");
            await _producer.ProduceAsync("demo", new Message<Null, string>()
            {
                Value = value
            }, cancellationToken);
        }
        _producer.Flush(TimeSpan.FromSeconds(10));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _producer.Dispose();
        return Task.CompletedTask;
    }
}