namespace DemoNotification.EmailSendService.Sources;

public interface IMessageConsumer
{
    Task Run<TMessage>(CancellationToken cancellationToken);
}
