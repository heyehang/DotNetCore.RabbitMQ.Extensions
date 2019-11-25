using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.RabbitMQ.Extensions
{
    public abstract class ConsumerService : IConsumerService
    {
        public abstract string Exchange { get; }
        public abstract string Queue { get; }

        public abstract string RoutingKey { get; }

        public abstract bool AutoAck { get; }

        public abstract string ServiceName { get; }

        ILogger logger;
        IConnectionChannelPool connectionChannelPool;
        public IModel channel;

        public ConsumerService(ILogger logger, IConnectionChannelPool connectionChannelPool)
        {
            this.connectionChannelPool = connectionChannelPool;
            this.logger = logger;
        }
        public virtual void Start()
        {
            channel = connectionChannelPool.Rent();

            channel.QueueDeclare(queue: Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Registered += Registered;
            consumer.Shutdown += Shutdown;
            consumer.Unregistered += Unregistered;
            consumer.Received += (s, e) =>
            {
                try
                {
                    Received(s, e);
                    if (!AutoAck)
                    {
                        channel.BasicAck(e.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{ServiceName}服务消费失败!");
                    if (!AutoAck)
                    {
                        channel.BasicReject(e.DeliveryTag, true);
                    }
                    throw;
                }
            };
            channel.BasicConsume(queue: Queue, autoAck: AutoAck, consumer: consumer);
        }

        public abstract void Received(object sender, BasicDeliverEventArgs e);
        public virtual void Registered(object sender, ConsumerEventArgs e)
        { }
        public virtual void Shutdown(object sender, ShutdownEventArgs e)
        { }
        public virtual void Unregistered(object sender, ConsumerEventArgs e)
        { }
    }
}
