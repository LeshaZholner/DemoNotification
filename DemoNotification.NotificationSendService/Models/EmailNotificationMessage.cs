namespace DemoNotification.NotificationSendService.Models;

public class EmailNotificationMessage
{
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
