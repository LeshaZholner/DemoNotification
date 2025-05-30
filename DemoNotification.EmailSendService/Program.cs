using DemoNotification.EmailSendService;
using DemoNotification.EmailSendService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<EmailBackgroundWorker>();
builder.Services.AddSingleton<KafkaConsumerService>();
builder.Services.AddSingleton<EmailSender>();

var host = builder.Build();
host.Run();
