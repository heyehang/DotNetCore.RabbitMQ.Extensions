using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestPublish : PublishService
    {
        public TestPublish(ILogger<TestPublish> logger, IConnectionChannelPool connectionChannelPool) : base(logger, connectionChannelPool)
        {
        }

        public override string ExchangeType => "direct";

        public override string Exchange => "test.ex";

        public override string Queue => "test.query";

        public override string RoutingKey => "test.key";

        public override string ServiceName => "TestPublish";
    }
}
