namespace DemoNotification.EmailSendService.Sources;

public interface IMessageHandler<TMessage>
{
    Task<bool> HandleAsync(TMessage message, CancellationToken cancellationToken);
}
