using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestDConsumer : ConsumerService
    {
        public TestDConsumer(ILogger<TestDConsumer> logger, IEnumerable<IConnectionChannelPool> connectionList) : base(logger, connectionList)
        {
        }

        public override string Queue => "test.query";

        public override bool AutoAck => true;

        public override string ServiceKey => nameof(TestDConsumer);

        public override string ConnectionKey => nameof(TestDConnection);

        public override void Received(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine($"消费者{ServiceKey}：收到消息：{e.Body.ToString()}");
        }
    }
}
