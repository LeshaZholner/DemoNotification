using DemoNotification.EmailSendService.Settings;
using Microsoft.Extensions.Options;

namespace DemoNotification.EmailSendService.Services;

public class EmailBackgroundWorker : BackgroundService
{
    private readonly ILogger<EmailBackgroundWorker> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly EmailSender _emailSender;
    private readonly KafkaConsumerService _kafkaConsumerService;

    public EmailBackgroundWorker(ILogger<EmailBackgroundWorker> logger,
        IConfiguration configuration,
        IOptions<KafkaSettings> kafkaOptinos,
        EmailSender emailSender,
        KafkaConsumerService kafkaConsumerService)
    {
        _logger = logger;
        _kafkaSettings = kafkaOptinos.Value;
        _emailSender = emailSender;
        _kafkaConsumerService = kafkaConsumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = _kafkaConsumerService.CreateConsumer();
        var topic = _kafkaSettings.Topic;

        consumer.Subscribe(topic);
        _logger.LogInformation("Subscribed to Kafka topic {Topic}", topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var message = _kafkaConsumerService.ParseMessage(result.Message.Value);

                if (message is not null)
                {
                    _logger.LogInformation("Processing message for {Email}", message.Email);
                    await _emailSender.SendEmailAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in EmailBackgroundWorker");
            }
        }
    }
}
