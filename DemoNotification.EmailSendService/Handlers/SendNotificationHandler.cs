using DemoNotification.EmailSendService.Models;
using DemoNotification.EmailSendService.Services;
using DemoNotification.EmailSendService.Sources;

namespace DemoNotification.EmailSendService.Handlers;

public class SendNotificationHandler : IMessageHandler<NotificationMessage>
{
    private readonly ILogger<SendNotificationHandler> _logger;
    private readonly IEmailSender _emailSender;

    public SendNotificationHandler(ILogger<SendNotificationHandler> logger, IEmailSender emailSender)
    {
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<bool> HandleAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing message for {Email}", message.Email);
            await _emailSender.SendEmailAsync(message);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in SendNotificationHandler {errorMessage}", ex.Message);
            return false;
        }
    }
}
