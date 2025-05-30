using DemoNotification.WebAPI.Models;

namespace DemoNotification.WebAPI.Services;

public interface INotificationQueueService
{
    Task EnqueueAsync(NotificationRequest notificationRequest);
}
