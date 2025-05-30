using Confluent.Kafka;
using DemoNotification.WebAPI.Models;
using DemoNotification.WebAPI.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DemoNotification.WebAPI.Services;

public class KafkaNotificationQueueService : INotificationQueueService
{
    private readonly IProducer<Null, string> _producer;
    private readonly KafkaSettings _kafkaSettings;

    public KafkaNotificationQueueService(IProducer<Null, string> producer, IOptions<KafkaSettings> kafkaOptions)
    {
        _producer = producer;
        _kafkaSettings = kafkaOptions.Value;
    }

    public async Task EnqueueAsync(NotificationRequest notificationRequest)
    {
        var notificationJson = JsonSerializer.Serialize(notificationRequest);
        await _producer.ProduceAsync(_kafkaSettings.Topic, new Message<Null, string>
        {
            Value = notificationJson
        });
    }
}
