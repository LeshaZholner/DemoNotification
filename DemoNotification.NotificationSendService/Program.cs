using DemoNotification.Kafka;
using DemoNotification.NotificationSendService;
using DemoNotification.NotificationSendService.Handlers;
using DemoNotification.NotificationSendService.Models;
using DemoNotification.NotificationSendService.Services;
using DemoNotification.NotificationSendService.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddConsumer<EmailNotificationMessage, EmailNotificationHandler>(builder.Configuration.GetSection("KafkaSettings"));
builder.Services.AddHostedService<MessageBackgroundService<EmailNotificationMessage>>();


var host = builder.Build();
host.Run();
