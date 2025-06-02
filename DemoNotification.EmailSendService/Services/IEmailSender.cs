using DemoNotification.EmailSendService.Models;

namespace DemoNotification.EmailSendService.Services;

public interface IEmailSender
{
    Task SendEmailAsync(NotificationMessage notificationMessage);
}
