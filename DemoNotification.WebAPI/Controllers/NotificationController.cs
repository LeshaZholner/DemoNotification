using DemoNotification.Core.Entities;
using DemoNotification.Kafka.Producer;
using DemoNotification.WebAPI.Models;
using DemoNotification.WebAPI.Models.Messages;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DemoNotification.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    private readonly IValidator<NotificationRequest> _validator;
    private readonly IMessageProducer<TelegramNotificationMessage> _tgMessageProducer;
    private readonly IMessageProducer<EmailNotificationMessage> _emailMessageProducer;

    public NotificationController(ILogger<NotificationController> logger,
        IValidator<NotificationRequest> validator,
        IMessageProducer<TelegramNotificationMessage> tgMessageProducer,
        IMessageProducer<EmailNotificationMessage> emailMessageProducer)
    {
        _logger = logger;
        _validator = validator;
        _tgMessageProducer = tgMessageProducer;
        _emailMessageProducer = emailMessageProducer;
    }

    [HttpPost]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed: {@Erros}", validationResult.Errors);
            return BadRequest(new
            {
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        _logger.LogInformation("Email -> Queuing notification to {Email} with subject {Subject}", request.Email, request.Subject);
        await _emailMessageProducer.ProduceAsync(new EmailNotificationMessage
        {
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message
        }, CancellationToken.None);

        _logger.LogInformation("TG -> Queuing notification to {ChatId}", request.ChatId);
        await _tgMessageProducer.ProduceAsync(new TelegramNotificationMessage
        {
            ChatId = request.ChatId,
            Message = request.Message
        }, CancellationToken.None);

        return Accepted(new
        {
            Message = "Notification has been queued successfull",
            request.Email,
            request.Subject,
        });
    }

    [HttpPost("{notificationDefinitionId:guid}")]
    public async Task<IActionResult> SendNotificationForm([FromBody] NotificationFormRequest request, Guid notificationDefinitionId)
    {
        var notificationDefinition = new NotificationDefinition
        {
            Id = notificationDefinitionId,
            Name = "Test Notification",
            Description = "Notification for testing",
            Fields =
            [
                new NotificationFieldDefinition { Id = Guid.Parse("CFDB033C-1DB1-4798-8E91-92E3407B57CC"), Name = "userName" },
                new NotificationFieldDefinition { Id = Guid.Parse("6381B544-F350-445C-893C-E996878A54A2"), Name = "email" },
                new NotificationFieldDefinition { Id = Guid.Parse("BE7DB890-D52B-4260-8809-A0D2E69EE7E1"), Name = "comment" }
            ],
            Channels =
            [
                new NotificationChannel 
                {
                    Id = Guid.Parse("E484CAC9-4282-4A8E-B81F-E81F971F676A"),
                    NotificationDefinitionId = notificationDefinitionId,
                    Name = "Email Channel",
                    Description = "Channel for sending to Email",
                    ChannelType = 1,
                    ConfigJson = @"{ ""subject"": ""Dear {{userName}}"", ""email"": ""lesha4dev@gmail.com"" }"
                },
                new NotificationChannel
                {
                    Id = Guid.Parse("A249686C-1677-41FF-B918-4BD6735F4A9F"),
                    NotificationDefinitionId = notificationDefinitionId,
                    Name = "TG Channel",
                    Description = "Channel for sending to TG",
                    ChannelType = 2,
                    ConfigJson = @"{ ""chatId"": ""-4744777748"" }"
                }
            ]
        };

        var templates = new List<ChannelTemplate>
        {
            new() {
                Id = Guid.Parse("E0AAC67D-37F6-4135-8B55-5CD8F1444EF0"),
                NotificationChannelId = Guid.Parse("E484CAC9-4282-4A8E-B81F-E81F971F676A"),
                Template = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Notification Email</title>\r\n  <style>\r\n    body {\r\n      font-family: Arial, sans-serif;\r\n      background-color: #f4f4f4;\r\n      margin: 0;\r\n      padding: 0;\r\n    }\r\n    .email-container {\r\n      background-color: #ffffff;\r\n      max-width: 600px;\r\n      margin: 30px auto;\r\n      padding: 20px;\r\n      border-radius: 8px;\r\n      box-shadow: 0 0 10px rgba(0,0,0,0.1);\r\n    }\r\n    .header {\r\n      font-size: 24px;\r\n      font-weight: bold;\r\n      color: #333333;\r\n      margin-bottom: 20px;\r\n    }\r\n    .field-label {\r\n      font-weight: bold;\r\n      color: #555555;\r\n    }\r\n    .field-value {\r\n      margin-bottom: 15px;\r\n      color: #333333;\r\n    }\r\n    .footer {\r\n      font-size: 12px;\r\n      color: #888888;\r\n      text-align: center;\r\n      margin-top: 30px;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-container\">\r\n    <div class=\"header\">New Comment Notification</div>\r\n    \r\n    <div class=\"field\">\r\n      <div class=\"field-label\">User Name:</div>\r\n      <div class=\"field-value\">{{userName}}</div>\r\n    </div>\r\n    \r\n    <div class=\"field\">\r\n      <div class=\"field-label\">Email:</div>\r\n      <div class=\"field-value\">{{email}}</div>\r\n    </div>\r\n    \r\n    <div class=\"field\">\r\n      <div class=\"field-label\">Comment:</div>\r\n      <div class=\"field-value\">{{comment}}</div>\r\n    </div>\r\n\r\n    <div class=\"footer\">\r\n      This is an automated message. Please do not reply.\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>"
            },
            new() {
                Id = Guid.Parse("097483E2-2455-497B-9B53-CCFCAC105473"),
                NotificationChannelId = Guid.Parse("A249686C-1677-41FF-B918-4BD6735F4A9F"),
                Template = "📬 Новое уведомление\r\n\r\n👤 Имя пользователя: {{userName}}\r\n📧 Email: {{email}}\r\n💬 Комментарий:\r\n{{comment}}"
            }
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var emailChannel = notificationDefinition.Channels.FirstOrDefault(x => x.ChannelType  == 1);

        if (emailChannel is not null)
        {
            var config = JsonSerializer.Deserialize<EmailConfig>(emailChannel.ConfigJson, jsonOptions);
            var template = templates.FirstOrDefault(template => template.NotificationChannelId == emailChannel.Id);
            var fields = notificationDefinition.Fields;

            if (template is not null && config is not null)
            {
                var message = Regex.Replace(template.Template, "{{(.*?)}}", match =>
                {
                    var key = match.Groups[1].Value;
                    return request.Fields.TryGetValue(key, out var value) ? value : string.Empty;
                });

                var subject = Regex.Replace(config.Subject, "{{(.*?)}}", match =>
                {
                    var key = match.Groups[1].Value;
                    return request.Fields.TryGetValue(key, out var value) ? value : string.Empty;
                });

                var emailNotificationMessage = new EmailNotificationMessage
                {
                    Email = config.Email,
                    Subject = subject,
                    Message = message
                };

                _logger.LogInformation("Email -> Queuing notification to {Email} with subject {Subject}", emailNotificationMessage.Email, emailNotificationMessage.Subject);
                await _emailMessageProducer.ProduceAsync(emailNotificationMessage, CancellationToken.None);
            }
        }

        var tgChannel = notificationDefinition.Channels.FirstOrDefault(x => x.ChannelType == 2);

        if (tgChannel is not null)
        {
            var config = JsonSerializer.Deserialize<TgConfig>(tgChannel.ConfigJson, jsonOptions);
            var template = templates.FirstOrDefault(template => template.NotificationChannelId == tgChannel.Id);
            var fields = notificationDefinition.Fields;

            if (template is not null && config is not null)
            {
                var message = Regex.Replace(template.Template, "{{(.*?)}}", match =>
                {
                    var key = match.Groups[1].Value;
                    return request.Fields.TryGetValue(key, out var value) ? value : string.Empty;
                });

                var telegramNotificationMessage = new TelegramNotificationMessage
                {
                    ChatId = config.ChatId,
                    Message = message
                };

                _logger.LogInformation("TG -> Queuing notification to {ChatId}", telegramNotificationMessage.ChatId);
                await _tgMessageProducer.ProduceAsync(telegramNotificationMessage, CancellationToken.None);
            }
        }

        return Accepted(new
        {
            Message = "Notification has been queued successfull"
        });
    }

    public class EmailConfig
    {
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }

    public class TgConfig
    {
        public string ChatId { get; set; } = string.Empty;
    }
}
