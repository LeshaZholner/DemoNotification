using DemoNotification.Kafka.Consumer;
using DemoNotification.NotificationSendService.Models;
using DemoNotification.NotificationSendService.Services;

namespace DemoNotification.NotificationSendService.Handlers;

public class EmailNotificationHandler : IMessageHandler<EmailNotificationMessage>
{
    private readonly ILogger<EmailNotificationHandler> _logger;
    private readonly IEmailSender _emailSender;

    public EmailNotificationHandler(ILogger<EmailNotificationHandler> logger, IEmailSender emailSender)
    {
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task HandleAsync(EmailNotificationMessage message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Email -> Processing message for {Email}", message.Email);
            await _emailSender.SendEmailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email -> Unexpected error in EmailNotificationHandler {errorMessage}", ex.Message);
        }
    }
}
