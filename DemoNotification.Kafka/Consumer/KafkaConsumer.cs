using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace DemoNotification.Kafka.Consumer;

public class KafkaConsumer<TMessage> : IMessageConsumer<TMessage>
{
    private readonly IMessageHandler<TMessage> _messageHandler;
    private readonly IConsumer<string, TMessage> _consumer;
    private readonly KafkaSettings _kafkaSettings;

    public KafkaConsumer(IMessageHandler<TMessage> messageHandler, IOptionsMonitor<KafkaSettings> kafkaSettings)
    {
        _messageHandler = messageHandler;
        _kafkaSettings = kafkaSettings.Get(typeof(TMessage).Name);
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<string, TMessage>(config)
            .SetValueDeserializer(new KafkaValueDeserializer<TMessage>())
            .Build();
    }

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_kafkaSettings.Topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumerResult = _consumer.Consume(cancellationToken);
                await _messageHandler.HandleAsync(consumerResult.Message.Value, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    public void Stop()
    {
        _consumer.Close();
    }
}
