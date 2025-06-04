using DemoNotification.Kafka.Consumer;

namespace DemoNotification.NotificationSendService;

public class MessageBackgroundService<TMessage> : BackgroundService
{
    private readonly IMessageConsumer<TMessage> _consumer;

    public MessageBackgroundService(IMessageConsumer<TMessage> consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => _consumer.ConsumeAsync(stoppingToken));
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Stop();
        return base.StopAsync(cancellationToken);
    }
}
