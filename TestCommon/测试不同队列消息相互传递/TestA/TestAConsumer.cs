using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TestCommon
{
    public class TestAConsumer : ConsumerService
    {
        TestBPublish testBPublish;

        public override string Queue => "test.query";

        public override string ServiceKey => nameof(TestAConsumer);

        public override bool AutoAck => false;

        public override string ConnectionKey => nameof(TestAConnection);

        private int count = 1;

        public TestAConsumer(ILogger<TestAConsumer> logger, IEnumerable<IConnectionChannelPool> connectionList, TestBPublish testBPublish) : base(logger, connectionList)
        {
            this.testBPublish = testBPublish;
        }

        public override void Received(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine($"{ServiceKey}收到第{count++}消息：{Encoding.UTF8.GetString(e.Body)}即将转发给testB消费");

            testBPublish.Publish($"{Encoding.UTF8.GetString(e.Body)}");
        }
    }
}
