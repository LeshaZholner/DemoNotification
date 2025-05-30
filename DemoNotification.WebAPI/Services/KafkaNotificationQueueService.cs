using Confluent.Kafka;
using DemoNotification.WebAPI.Models;
using System.Text.Json;

namespace DemoNotification.WebAPI.Services;

public class KafkaNotificationQueueService : INotificationQueueService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaNotificationQueueService(IProducer<Null, string> producer, IConfiguration configuration)
    {
        _producer = producer;
        _topic = configuration["Kafka:Topic"] ?? "notification-topic";
    }

    public async Task EnqueueAsync(NotificationRequest notificationRequest)
    {
        var notificationJson = JsonSerializer.Serialize(notificationRequest);
        await _producer.ProduceAsync(_topic, new Message<Null, string>
        {
            Value = notificationJson
        });
    }
}
