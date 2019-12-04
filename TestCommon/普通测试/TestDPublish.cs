using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestDPublish : PublishService
    {
        public TestDPublish(ILogger<TestDPublish> logger, IEnumerable<IConnectionChannelPool> connectionList) : base(logger, connectionList)
        {
        }

        public override string ExchangeType => "direct";

        public override string Exchange => "testd.ex";

        public override string Queue => "testd.query";

        public override string RoutingKey => "testd.key";

        public override string ConnectionKey => nameof(TestDConnection);

        public override string ServiceKey => nameof(TestDPublish);
    }
}
