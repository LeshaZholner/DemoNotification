using Confluent.Kafka;
using System.Text.Json;

namespace DemoNotification.Kafka.Producer;

public class KafkaValueSerializer<TMessage> : ISerializer<TMessage>
{
    public byte[] Serialize(TMessage data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}
