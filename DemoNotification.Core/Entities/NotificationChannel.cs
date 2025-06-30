namespace DemoNotification.Core.Entities;

public class NotificationChannel
{
    public Guid Id { get; set; }
    public Guid NotificationDefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ConfigJson { get; set; } = string.Empty;
    public int ChannelType { get; set; }
}
