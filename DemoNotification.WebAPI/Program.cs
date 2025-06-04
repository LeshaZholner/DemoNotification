using DemoNotification.Kafka;
using DemoNotification.WebAPI.Extensions;
using DemoNotification.WebAPI.Models;

var builder = WebApplication.CreateBuilder(args);


//builder.Logging.ClearProviders();
//builder.Logging.AddOpenTelemetry(x => x.AddOtlpExporter());

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddProducer<NotificationRequest>(builder.Configuration.GetSection("KafkaSettings"));
builder.Services.AddModelsValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
