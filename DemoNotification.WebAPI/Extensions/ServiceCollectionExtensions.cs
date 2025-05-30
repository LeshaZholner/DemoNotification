using Confluent.Kafka;
using DemoNotification.WebAPI.Services;
using DemoNotification.WebAPI.Settings;
using DemoNotification.WebAPI.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace DemoNotification.WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaNotificationQueue(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = sp.GetService<IOptions<KafkaSettings>>()!.Value.BootstrapServers
            };

            return new ProducerBuilder<Null, string>(config).Build();
        });

        return services;
    }

    public static IServiceCollection AddNotificationQueue(this IServiceCollection services)
    {
        services.AddScoped<INotificationQueueService, KafkaNotificationQueueService>();
        return services;
    }

    public static IServiceCollection AddModelsValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<NotificationRequestValidator>();

        return services;
    }
}
