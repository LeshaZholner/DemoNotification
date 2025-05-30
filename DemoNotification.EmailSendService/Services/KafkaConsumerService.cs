using Confluent.Kafka;
using DemoNotification.EmailSendService.Models;
using DemoNotification.EmailSendService.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DemoNotification.EmailSendService.Services;

public class KafkaConsumerService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IOptions<KafkaSettings> kafkaOptions)
    {
        _logger = logger;
        _kafkaSettings = kafkaOptions.Value;
    }

    public IConsumer<Ignore, string> CreateConsumer()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        return new ConsumerBuilder<Ignore, string>(config).Build();
    }

    public NotificationMessage? ParseMessage(string message)
    {
        try
        {
            return JsonSerializer.Deserialize<NotificationMessage>(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error to parse Kafka message: {message}", message);
            return null;
        }
    }
}
