using DemoNotification.Kafka;
using DemoNotification.NotificationSendService;
using DemoNotification.NotificationSendService.Handlers;
using DemoNotification.NotificationSendService.Models;
using DemoNotification.NotificationSendService.Services;
using DemoNotification.NotificationSendService.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection(nameof(TelegramSettings)));
builder.Services.AddSingleton<ITelegramSender, TelegramSender>();

builder.Services.AddConsumer<EmailNotificationMessage, EmailNotificationHandler>(builder.Configuration.GetSection("KafkaEmailSettings"));
builder.Services.AddHostedService<MessageBackgroundService<EmailNotificationMessage>>();

builder.Services.AddConsumer<TelegramNotificationMessage, TelegramNotificationHandler>(builder.Configuration.GetSection("KafkaTgSettings"));
builder.Services.AddHostedService<MessageBackgroundService<TelegramNotificationMessage>>();


var host = builder.Build();
host.Run();
