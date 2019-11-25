using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.RabbitMQ.Extensions
{
    public abstract class PublishService : IPublishService
    {
        public abstract string ExchangeType { get; }
        public abstract string Exchange { get; }
        public abstract string Queue { get; }

        public abstract string RoutingKey { get; }

        public abstract string ServiceName { get; }

        ILogger logger;
        IConnectionChannelPool connectionChannelPool;

        public PublishService(ILogger logger, IConnectionChannelPool connectionChannelPool)
        {
            this.connectionChannelPool = connectionChannelPool;
            this.logger = logger;
        }

        public void Publish(object objmsg)
        {
            var channel = connectionChannelPool.Rent();
            try
            {
                var body = Encoding.UTF8.GetBytes(objmsg?.ToString());
                channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType, true);
                channel.QueueDeclare(queue: Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                channel.QueueBind(queue: Queue, exchange: Exchange, routingKey: RoutingKey);

                channel.BasicPublish(exchange: Exchange, routingKey: RoutingKey, properties, body);
            }
            catch (Exception ex)
            {
                logger.LogError($"{ServiceName}发送消息出错：{ex.StackTrace}");
                throw;
            }
            finally
            {
                var returned = connectionChannelPool.Return(channel);
                if (!returned)
                {
                    channel.Dispose();
                }
            }
        }

        public Task PublishAsync(object objmsg)
        {
            var channel = connectionChannelPool.Rent();
            try
            {
                var body = Encoding.UTF8.GetBytes(objmsg?.ToString());
                channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType, true);
                channel.QueueDeclare(queue: Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                channel.QueueBind(queue: Queue, exchange: Exchange, routingKey: RoutingKey);

                channel.BasicPublish(exchange: Exchange, routingKey: RoutingKey, properties, body);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError($"{ServiceName}发送消息出错：{ex.StackTrace}");
                throw;
            }
            finally
            {
                var returned = connectionChannelPool.Return(channel);
                if (!returned)
                {
                    channel.Dispose();
                }
            }
        }
    }
}
