using DemoNotification.EmailSendService.Services;
using DemoNotification.EmailSendService.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));

builder.Services.AddHostedService<EmailBackgroundWorker>();
builder.Services.AddSingleton<KafkaConsumerService>();
builder.Services.AddSingleton<EmailSender>();

var host = builder.Build();
host.Run();
