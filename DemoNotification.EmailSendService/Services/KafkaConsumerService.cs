using Confluent.Kafka;
using DemoNotification.EmailSendService.Models;
using System.Text.Json;

namespace DemoNotification.EmailSendService.Services;

public class KafkaConsumerService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConfiguration _configuration;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IConsumer<Ignore, string> CreateConsumer()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = _configuration["Kafka:GroupId"],
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
