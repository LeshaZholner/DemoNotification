namespace DemoNotification.Core.Enums;

public enum OutboxStatus
{
    Pending,
    Processing,
    Succeded,
    Failed,
    Skiped,
    DeadLettered
}
