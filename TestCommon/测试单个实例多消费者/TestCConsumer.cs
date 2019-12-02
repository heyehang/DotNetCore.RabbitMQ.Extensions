using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TestCommon
{
    public class TestCConsumer : ConsumerService
    {
        public override string Queue => "test.query";

        public override string ServiceKey => "TestCConsumer";

        public override bool AutoAck => false;

        public override string ConnectionKey => "TestCconn";

        public override int ConsumerTotal => 10;

        private int count = 1;

        public TestCConsumer(ILogger<TestCConsumer> logger, IEnumerable<IConnectionChannelPool> connectionList) : base(logger, connectionList)
        {
        }

        public override void Received(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine($"{ServiceKey}收到testC第{count++}消息：{Encoding.UTF8.GetString(e.Body)}");
        }
    }
}
