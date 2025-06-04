using DemoNotification.NotificationSendService.Models;

namespace DemoNotification.NotificationSendService.Services;

public interface IEmailSender
{
    Task SendEmailAsync(EmailNotificationMessage notificationMessage);
}
