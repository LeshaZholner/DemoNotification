using DemoNotification.NotificationSendService.Models;
using DemoNotification.NotificationSendService.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Polly;

namespace DemoNotification.NotificationSendService.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly SmtpSettings _smtpSettings;
    private readonly AsyncPolicy _retryPolicy;

    public EmailSender(ILogger<EmailSender> logger, IOptions<SmtpSettings> smtpOptions)
    {
        _logger = logger;
        _smtpSettings = smtpOptions.Value;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttemt => TimeSpan.FromSeconds(Math.Pow(2, retryAttemt)),
                (ex, time) => _logger.LogWarning(ex, "Retrying email send in {Delay}", time));
    }

    public async Task SendEmailAsync(EmailNotificationMessage notificationMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("DemoNotification", _smtpSettings.From));
        message.To.Add(MailboxAddress.Parse(notificationMessage.Email));
        message.Subject = notificationMessage.Subject;

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = notificationMessage.Message,
        };

        await _retryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, false);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        });

        _logger.LogInformation("Email -> Message sent to {Recipient}", notificationMessage.Email);
    }
}
