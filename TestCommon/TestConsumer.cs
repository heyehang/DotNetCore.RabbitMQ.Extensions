using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestConsumer : ConsumerService
    {
        public TestConsumer(ILogger<TestConsumer> logger, IConnectionChannelPool connectionChannelPool) : base(logger, connectionChannelPool)
        {
        }

        public override string Exchange => "test.ex";

        public override string Queue => "test.query";

        public override string RoutingKey => "test.key";

        public override string ServiceName => "TestPublish";

        public override bool AutoAck => false;

        private int count = 1;
        public override void Received(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine($"收到第{count++}消息：{Encoding.UTF8.GetString(e.Body)}");
        }
    }
}
