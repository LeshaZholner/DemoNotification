using DemoNotification.EmailSendService.Models;
using DemoNotification.EmailSendService.Sources;

namespace DemoNotification.EmailSendService.BackgroundServices;

public class SendNotificationService : MessageBackgroundService<NotificationMessage>
{
    public SendNotificationService(ILogger<MessageBackgroundService<NotificationMessage>> logger, IMessageConsumer messageConsumer)
        : base(logger, messageConsumer)
    {
    }
}
