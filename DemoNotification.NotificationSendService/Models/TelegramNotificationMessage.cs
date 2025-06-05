namespace DemoNotification.NotificationSendService.Models;

public class TelegramNotificationMessage
{
    public string ChatId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
