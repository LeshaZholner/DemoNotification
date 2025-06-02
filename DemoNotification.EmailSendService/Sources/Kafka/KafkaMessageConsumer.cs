using Confluent.Kafka;
using DemoNotification.EmailSendService.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DemoNotification.EmailSendService.Sources.Kafka;

public class KafkaMessageConsumer : IMessageConsumer
{
    private readonly ILogger<KafkaMessageConsumer> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public KafkaMessageConsumer(ILogger<KafkaMessageConsumer> logger, IOptions<KafkaSettings> kafkaOptions, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _kafkaSettings = kafkaOptions.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Run<TMessage>(CancellationToken cancellationToken)
    {
        var kafkaConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<Ignore, string>(kafkaConfig).Build();
        consumer.Subscribe(_kafkaSettings.Topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumerResult = consumer.Consume(cancellationToken);
                var message = JsonSerializer.Deserialize<TMessage>(consumerResult.Message.Value);

                if (message is not null)
                {
                    await HandleMessage(message, cancellationToken);
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task<bool> HandleMessage<TMessage>(TMessage message, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();
            return await messageHandler.HandleAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Message consumer failed unexpectedly. {errorMessage}", ex.Message);
            return false;
        }
    }
}
