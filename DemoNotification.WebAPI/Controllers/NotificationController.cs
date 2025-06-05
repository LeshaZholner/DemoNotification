using DemoNotification.Kafka.Producer;
using DemoNotification.WebAPI.Models;
using DemoNotification.WebAPI.Models.Messages;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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
}
