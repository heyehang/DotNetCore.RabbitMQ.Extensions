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
        IEnumerable<IPublishService> testPublishList;

        public override string Queue => "test.query";

        public override string ServiceKey => "TestAConsumer";

        public override bool AutoAck => false;

        public override string ConnectionKey => "TestAconn";

        private int count = 1;

        public TestAConsumer(ILogger<TestAConsumer> logger, IEnumerable<IConnectionChannelPool> connectionList, IEnumerable<IPublishService> testPublishList) : base(logger, connectionList)
        {
            this.testPublishList = testPublishList;
        }

        public override void Received(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine($"{ServiceKey}收到第{count++}消息：{Encoding.UTF8.GetString(e.Body)}即将转发给testB消费");
            var testBPublish = testPublishList.FirstOrDefault(x => x.ServiceKey == "TestBPublish");
            testBPublish.Publish($"{Encoding.UTF8.GetString(e.Body)}");
        }
    }
}
