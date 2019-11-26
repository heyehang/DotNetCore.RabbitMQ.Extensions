using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetCore.RabbitMQ.Extensions
{
    public abstract class ConsumerService : IConnectionKey, IConsumerService
    {
        public abstract string Exchange { get; }
        public abstract string Queue { get; }

        public abstract string RoutingKey { get; }

        public abstract bool AutoAck { get; }

        public abstract string ServiceKey { get; }
        public abstract string ConnectionKey { get; }

        public virtual int ConsumerTotal { get; } = 1;
        ILogger logger;
        IEnumerable<IConnectionChannelPool> connectionList;
        public List<IModel> channelList = new List<IModel>();
        public ConsumerService(ILogger logger, IEnumerable<IConnectionChannelPool> connectionList)
        {
            this.connectionList = connectionList;
            this.logger = logger;
        }
        public virtual void Start()
        {
            var connectionChannelPool = connectionList.FirstOrDefault(e => e.ConnectionKey == ConnectionKey);
            if (connectionChannelPool == null)
            {
                throw new Exception($"{ServiceKey}未找到相应的ConnectionChannelPool,请确保ConnectionKey是否匹配实现");
            }
            for (int i = 0; i < ConsumerTotal; i++)
            {
                var channel = connectionChannelPool.Rent();

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
                        logger.LogError(ex, $"{ServiceKey}服务消费失败!");
                        if (!AutoAck)
                        {
                            channel.BasicReject(e.DeliveryTag, true);
                        }
                        throw;
                    }
                };
                channel.BasicConsume(queue: Queue, autoAck: AutoAck, consumer: consumer);
                channelList.Add(channel);
            }
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
