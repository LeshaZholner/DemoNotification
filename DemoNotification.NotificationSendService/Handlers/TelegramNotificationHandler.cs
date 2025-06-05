using DemoNotification.Kafka.Consumer;
using DemoNotification.NotificationSendService.Models;
using DemoNotification.NotificationSendService.Services;

namespace DemoNotification.NotificationSendService.Handlers;

public class TelegramNotificationHandler : IMessageHandler<TelegramNotificationMessage>
{
    private readonly ILogger<TelegramNotificationHandler> _logger;
    private readonly ITelegramSender _telegramSender;

    public TelegramNotificationHandler(ILogger<TelegramNotificationHandler> logger, ITelegramSender telegramSender)
    {
        _logger = logger;
        _telegramSender = telegramSender;
    }

    public async Task HandleAsync(TelegramNotificationMessage message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("TG -> Proccesing message for {ChatId}", message.ChatId);
            await _telegramSender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TG -> Unexpected error in TelegramNotificationHandler {errorMessage}", ex.Message);
        }
    }
}
