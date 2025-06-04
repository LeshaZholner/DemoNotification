namespace DemoNotification.Kafka.Producer;

public interface IMessageProducer<TMessage> : IDisposable
{
    Task ProduceAsync(TMessage message, CancellationToken cancellationToken);
}
