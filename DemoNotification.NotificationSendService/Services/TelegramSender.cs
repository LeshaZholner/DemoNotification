using DemoNotification.NotificationSendService.Models;
using DemoNotification.NotificationSendService.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace DemoNotification.NotificationSendService.Services;

public class TelegramSender : ITelegramSender
{
    private readonly ILogger<TelegramSender> _logger;
    private readonly TelegramSettings _telegramSettings;

    public TelegramSender(ILogger<TelegramSender> logger, IOptions<TelegramSettings> telegramSettings)
    {
        _logger = logger;
        _telegramSettings = telegramSettings.Value;
    }

    public async Task SendMessageAsync(TelegramNotificationMessage message)
    {
        var botClient = new TelegramBotClient(_telegramSettings.Token);
        await botClient.SendMessage(message.ChatId, message.Message);

        _logger.LogInformation("TG -> Message sent to {ChatId}", message.ChatId);
    }
}
