using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestBPublish : PublishService
    {
        public TestBPublish(ILogger<TestBPublish> logger, IEnumerable<IConnectionChannelPool> connectionList) : base(logger, connectionList)
        {
        }

        public override string ExchangeType => "direct";

        public override string Exchange => "test.ex";

        public override string Queue => "test.query";

        public override string RoutingKey => "test.key";

        public override string ServiceKey => "TestBPublish";

        public override string ConnectionKey => "TestBconn";
    }
}
