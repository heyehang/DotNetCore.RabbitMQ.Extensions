using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestEPublish : PublishService
    {
        public TestEPublish(ILogger<TestEPublish> logger, IEnumerable<IConnectionChannelPool> connectionList) : base(logger, connectionList)
        {
        }

        public override string ExchangeType => "direct";

        public override string Exchange => "testd.ex";

        public override string Queue => "teste.query";

        public override string RoutingKey => "teste.key";

        public override string ConnectionKey => nameof(TestDConnection);

        public override string ServiceKey => nameof(TestEPublish);

        public override void BasicAcks(object objmsg)
        {
            Console.WriteLine($"{nameof(TestEPublish)}发送消息成功！objmsg:{objmsg}");
        }
        public override void BasicNacks(object objmsg)
        {
            Console.WriteLine($"{nameof(TestEPublish)}发送消息失败！objmsg:{objmsg}");
        }
    }
}
