using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestAPublish : PublishService
    {
        public TestAPublish(ILogger<TestAPublish> logger, IEnumerable<IConnectionChannelPool> connectionList) : base(logger, connectionList)
        {
        }

        public override string ExchangeType => "direct";

        public override string Exchange => "test.ex";

        public override string Queue => "test.query";

        public override string RoutingKey => "test.key";

        public override string ServiceKey => nameof(TestAPublish);

        public override string ConnectionKey => nameof(TestAConnection);
    }
}
