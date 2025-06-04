using DemoNotification.Kafka.Producer;
using DemoNotification.WebAPI.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DemoNotification.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    private readonly IValidator<NotificationRequest> _validator;
    private readonly IMessageProducer<NotificationRequest> _messageProducer;

    public NotificationController(ILogger<NotificationController> logger,
        IValidator<NotificationRequest> validator,
        IMessageProducer<NotificationRequest> messageProducer)
    {
        _logger = logger;
        _validator = validator;
        _messageProducer = messageProducer;
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
        
        _logger.LogInformation("Queuing email to {Email} with subject {Subject}", request.Email, request.Subject);
        await _messageProducer.ProduceAsync(request, CancellationToken.None);

        return Accepted(new
        {
            Message = "Notification has been queued successfull",
            request.Email,
            request.Subject,
        });
    }
}
