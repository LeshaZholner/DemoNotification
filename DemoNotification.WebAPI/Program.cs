using DemoNotification.Kafka;
using DemoNotification.WebAPI.Extensions;
using DemoNotification.WebAPI.Models;
using DemoNotification.WebAPI.Models.Messages;

var builder = WebApplication.CreateBuilder(args);


//builder.Logging.ClearProviders();
//builder.Logging.AddOpenTelemetry(x => x.AddOtlpExporter());

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddProducer<EmailNotificationMessage>(builder.Configuration.GetSection("KafkaEmailSettings"));
builder.Services.AddProducer<TelegramNotificationMessage>(builder.Configuration.GetSection("KafkaTgSettings"));

builder.Services.AddModelsValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
