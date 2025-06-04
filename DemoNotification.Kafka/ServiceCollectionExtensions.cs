using DemoNotification.Kafka.Consumer;
using DemoNotification.Kafka.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoNotification.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsumer<TMessage, THandler>(
        this IServiceCollection serviceCollection, IConfigurationSection configurationSection)
        where THandler : class, IMessageHandler<TMessage>
    {
        serviceCollection.Configure<Consumer.KafkaSettings>(configurationSection);
        serviceCollection.AddSingleton<IMessageConsumer<TMessage>, KafkaConsumer<TMessage>>();
        serviceCollection.AddSingleton<IMessageHandler<TMessage>, THandler>();

        return serviceCollection;
    }

    public static IServiceCollection AddProducer<TMessage>(this IServiceCollection serviceCollection, IConfigurationSection configurationSection)
    {
        serviceCollection.Configure<Producer.KafkaSettings>(configurationSection);
        serviceCollection.AddSingleton<IMessageProducer<TMessage>, KafkaProducer<TMessage>>();

        return serviceCollection;
    }
}
