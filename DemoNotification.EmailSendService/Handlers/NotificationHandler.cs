using DemoNotification.EmailSendService.Models;
using DemoNotification.EmailSendService.Services;
using DemoNotification.Kafka.Consumer;

namespace DemoNotification.EmailSendService.Handlers;

public class NotificationHandler : IMessageHandler<NotificationMessage>
{
    private readonly ILogger<NotificationHandler> _logger;
    private readonly IEmailSender _emailSender;

    public NotificationHandler(ILogger<NotificationHandler> logger, IEmailSender emailSender)
    {
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task HandleAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing message for {Email}", message.Email);
            await _emailSender.SendEmailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in SendNotificationHandler {errorMessage}", ex.Message);
        }
    }
}
