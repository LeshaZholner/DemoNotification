using DemoNotification.WebAPI.Extensions;
using DemoNotification.WebAPI.Settings;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);


//builder.Logging.ClearProviders();
//builder.Logging.AddOpenTelemetry(x => x.AddOtlpExporter());

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));

builder.Services.AddKafkaNotificationQueue(builder.Configuration);
builder.Services.AddNotificationQueue();
builder.Services.AddModelsValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
