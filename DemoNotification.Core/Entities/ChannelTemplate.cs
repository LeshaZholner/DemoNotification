namespace DemoNotification.Core.Entities;

public class ChannelTemplate
{
    public Guid Id { get; set; }
    public Guid NotificationChannelId { get; set; }
    public string Template { get; set; } = string.Empty;
}
