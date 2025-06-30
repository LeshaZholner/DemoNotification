namespace DemoNotification.Core.Entities;

public class NotificationFieldDefinition
{
    public Guid Id { get; set; }
    public Guid NotificationDefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
}
