using DemoNotification.EmailSendService;
using DemoNotification.EmailSendService.Handlers;
using DemoNotification.EmailSendService.Models;
using DemoNotification.EmailSendService.Services;
using DemoNotification.EmailSendService.Settings;
using DemoNotification.Kafka;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddConsumer<NotificationMessage, NotificationHandler>(builder.Configuration.GetSection("KafkaSettings"));
builder.Services.AddHostedService<MessageBackgroundService<NotificationMessage>>();

var host = builder.Build();
host.Run();
