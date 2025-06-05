namespace DemoNotification.WebAPI.Models.Messages;

public class TelegramNotificationMessage
{
    public string ChatId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
