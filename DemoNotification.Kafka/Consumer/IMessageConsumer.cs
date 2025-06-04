namespace DemoNotification.Kafka.Consumer;

public interface IMessageConsumer<TMessage>
{
    Task ConsumeAsync(CancellationToken cancellationToken);
    void Stop();
}
