using Confluent.Kafka;
using System.Text.Json;

namespace DemoNotification.Kafka.Consumer;

public class KafkaValueDeserializer<TMessage> : IDeserializer<TMessage>
{
    public TMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<TMessage>(data)!;
    }
}
