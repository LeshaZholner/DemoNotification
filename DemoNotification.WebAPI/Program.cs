using DemoNotification.WebAPI.Extensions;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);


//builder.Logging.ClearProviders();
//builder.Logging.AddOpenTelemetry(x => x.AddOtlpExporter());

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddKafkaNotificationQueue(builder.Configuration);
builder.Services.AddNotificationQueue();
builder.Services.AddModelsValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
