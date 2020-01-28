using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.RabbitMQ.Extensions
{
    public abstract class PublishService : IConnectionKey, IPublishService
    {
        ILogger logger;
        IConnectionChannelPool connectionChannelPool;
        public PublishService(ILogger logger, IEnumerable<IConnectionChannelPool> connectionList)
        {
            this.logger = logger;
            connectionChannelPool = connectionList.FirstOrDefault(e => e.ConnectionKey == ConnectionKey);
            if (connectionChannelPool == null)
            {
                throw new Exception($"{ServiceKey}未找到相应的ConnectionChannelPool,请确保ConnectionKey是否匹配实现");
            }
        }
        public abstract string ExchangeType { get; }
        public abstract string Exchange { get; }
        public abstract string Queue { get; }

        public abstract string RoutingKey { get; }

        public abstract string ConnectionKey { get; }
        public abstract string ServiceKey { get; }

        public virtual void BasicAcks(object objmsg)
        {
        }
        public virtual void BasicNacks(object objmsg)
        {
        }

        public void Publish(object objmsg)
        {
            var channel = connectionChannelPool.Rent();

            try
            {
                channel.ConfirmSelect();
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objmsg));

                channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType, true);
                channel.QueueDeclare(queue: Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue: Queue, exchange: Exchange, routingKey: RoutingKey);

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                channel.BasicPublish(exchange: Exchange, routingKey: RoutingKey, mandatory: true, basicProperties: properties, body: body);

                if (channel.WaitForConfirms()) BasicAcks(objmsg); else BasicNacks(objmsg);
            }
            catch (Exception ex)
            {
                logger.LogError($"{ServiceKey}发送消息出错：{ex.StackTrace}");
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
                channel.ConfirmSelect();
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objmsg));

                channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType, true);
                channel.QueueDeclare(queue: Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue: Queue, exchange: Exchange, routingKey: RoutingKey);

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                channel.BasicPublish(exchange: Exchange, routingKey: RoutingKey, mandatory: true, basicProperties: properties, body: body);

                if (channel.WaitForConfirms()) BasicAcks(objmsg); else BasicNacks(objmsg);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError($"{ServiceKey}发送消息出错：{ex.StackTrace}");
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
