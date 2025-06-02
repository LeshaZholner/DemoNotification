using DemoNotification.EmailSendService.Sources;

namespace DemoNotification.EmailSendService.BackgroundServices;

public class MessageBackgroundService<TMessage> : BackgroundService
{
    private readonly ILogger<MessageBackgroundService<TMessage>> _logger;
    private readonly IMessageConsumer _messageConsumer;

    public MessageBackgroundService(ILogger<MessageBackgroundService<TMessage>> logger, IMessageConsumer messageConsumer)
    {
        _logger = logger;
        _messageConsumer = messageConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _messageConsumer.Run<TMessage>(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Message consumer failed unexpectedly. {errorMessage}", ex.Message);
            throw;
        }
    }
}
