using DemoNotification.Core.Enums;

namespace DemoNotification.Core.Entities;

public class OutboxNotification
{
    public Guid Id { get; set; }
    public Guid NotificationChannelId { get; set; }
    public string FieldJson { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public OutboxStatus Status { get; set; }
    public DateTime CreatedAd { get; set; }
}
