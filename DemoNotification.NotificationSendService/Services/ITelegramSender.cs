using DemoNotification.NotificationSendService.Models;

namespace DemoNotification.NotificationSendService.Services;

public interface ITelegramSender
{
    Task SendMessageAsync(TelegramNotificationMessage message);
}
