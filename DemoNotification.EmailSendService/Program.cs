using DemoNotification.EmailSendService.BackgroundServices;
using DemoNotification.EmailSendService.Handlers;
using DemoNotification.EmailSendService.Models;
using DemoNotification.EmailSendService.Services;
using DemoNotification.EmailSendService.Settings;
using DemoNotification.EmailSendService.Sources;
using DemoNotification.EmailSendService.Sources.Kafka;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));

builder.Services.AddHostedService<SendNotificationService>();
builder.Services.AddSingleton<IMessageConsumer, KafkaMessageConsumer>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddScoped<IMessageHandler<NotificationMessage>, SendNotificationHandler>();

var host = builder.Build();
host.Run();
