using DemoNotification.EmailSendService.Models;
using MailKit.Net.Smtp;
using MimeKit;
using Polly;

namespace DemoNotification.EmailSendService.Services;

public class EmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly IConfiguration _configuration;
    private readonly AsyncPolicy _retryPolicy;

    public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttemt => TimeSpan.FromSeconds(Math.Pow(2, retryAttemt)),
                (ex, time) => _logger.LogWarning(ex, "Retrying email send in {Delay}", time));
    }

    public async Task SendEmailAsync(NotificationMessage notificationMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("DemoNotification", _configuration["Smtp:From"]));
        message.To.Add(MailboxAddress.Parse(notificationMessage.Email));
        message.Subject = notificationMessage.Subject;

        message.Body = new TextPart("plain")
        {
            Text = notificationMessage.Message,
        };

        _logger.LogInformation("Config: {Host}, {Port}, {UserName}, {Password}", _configuration["Smtp:Host"], _configuration["Smtp:Port"], _configuration["Smtp:Username"], _configuration["Smtp:Password"]);

        await _retryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), false);
            await client.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        });

        _logger.LogInformation("Email sent to {Recipient}", notificationMessage.Email);
    }
}
