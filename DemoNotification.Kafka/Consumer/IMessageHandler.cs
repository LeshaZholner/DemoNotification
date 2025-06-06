﻿namespace DemoNotification.Kafka.Consumer;

public interface IMessageHandler<TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken);
}
