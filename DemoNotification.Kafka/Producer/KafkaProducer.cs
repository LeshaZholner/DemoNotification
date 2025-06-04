using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace DemoNotification.Kafka.Producer;

public class KafkaProducer<TMessage> : IMessageProducer<TMessage>
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly IProducer<string, TMessage> _producer;

    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings)
    {
        _kafkaSettings = kafkaSettings.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers
        };

        _producer = new ProducerBuilder<string, TMessage>(config)
            .SetValueSerializer(new KafkaValueSerializer<TMessage>())
            .Build();

    }

    public async Task ProduceAsync(TMessage message, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(_kafkaSettings.Topic, new Message<string, TMessage>
        {
            Key = "uniq1",
            Value = message
        }, cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _producer.Dispose();
        }
    }
}
