namespace DemoNotification.Core.Entities;

public class NotificationDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<NotificationFieldDefinition>? Fields { get; set; }
    public IEnumerable<NotificationChannel>? Channels { get; set; }
}
